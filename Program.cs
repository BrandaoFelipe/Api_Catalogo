using APICatalogo.Context;
using APICatalogo.DTO.Mappings;
using APICatalogo.Extensions;
using APICatalogo.Filters;
using APICatalogo.Models;
using APICatalogo.RateLimitOptions;
using APICatalogo.Repositories;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers(options =>
        {
            options.Filters.Add(typeof(ApiExceptionFilter));
        })
            .AddJsonOptions(options =>
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles).AddNewtonsoftJson();

        //INICIO CORS
        var origensComAcessoPermitido = "_origensComAcessoPermitido";
        builder.Services.AddCors(options =>
        {
            options.AddPolicy(name: origensComAcessoPermitido,
                policy =>
                {
                    //policy.AllowAnyOrigin();
                    policy.WithOrigins("https://localhost:7016")
                    .AllowAnyMethod()
                    .AllowAnyHeader();
                    //.WithMethods("GET", "POST");
                });
        });
        //FINAL CORS

        builder.Services.AddEndpointsApiExplorer();

        //DOCUMENTAÇÃO
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "apicatalogo", Version = "v1" });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Bearer JWT"
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[] { }
                }
            });
        });

        builder.Services.AddIdentity<ApplicationUser, IdentityRole>(). //ApplicationUser(que está herdando de IdentityUser) é a classe para representar os usuários,
                                                                       //IdentityRole classe para as funções relacionado aos perfis
            AddEntityFrameworkStores<AppDbContext>() //mecanismo para armazenar os dados relacionado a classe de context
            .AddDefaultTokenProviders(); //provedores de tokens padrão para lidar com as operações de autenticação.

        string? mySqlConnection = builder.Configuration.GetConnectionString("DefaultConnection"); //String de conexão com o MySQL
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseMySql(mySqlConnection,
                ServerVersion.AutoDetect(mySqlConnection)));

        //INICIO CÓDIGO DE AUTENTICAÇÃO BEARER
        var secretKey = builder.Configuration["JWT:SecretKey"]

                ?? throw new ArgumentException("Invalid ultra secret key!");

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

        }).AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.Zero,
                ValidAudience = builder.Configuration["JWT:ValidAudience"],
                ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(secretKey))
            };
        }); //FINAL CODIGO DE AUTENTICAÇÃO BEARER


        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("SuperAdminOnly", policy => policy.RequireRole("SuperAdmin").RequireClaim("id", "master"));
            options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
            options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));
            options.AddPolicy("ExclusePolicyOnly", policy => policy.RequireAssertion(context =>
                             context.User.HasClaim(claim =>
                                                   claim.Type == "id" && claim.Value == "master") || context.User.IsInRole("SuperAdmin")));
        });
        builder.Services.AddApiVersioning(o =>
        {
            o.DefaultApiVersion = new ApiVersion(1, 0);
            o.AssumeDefaultVersionWhenUnspecified = true;
            o.ReportApiVersions = true;
            o.ApiVersionReader = ApiVersionReader.Combine(new QueryStringApiVersionReader(),
                                                            new UrlSegmentApiVersionReader());
            //Quando não explicitado o esquema de utilização, por padrão utiliza-se o QueryString

        }).AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV"; //formata a versão com "'v'major[.minor][-status]"
            options.SubstituteApiVersionInUrl = true;
        });

        //RateLimiting INICIO
        var myOptions = new MyRateLimitOptions();
        builder.Configuration.GetSection(MyRateLimitOptions.MyRateLimit).Bind(myOptions);


        builder.Services.AddRateLimiter(rateLimitOptions =>
        {
            rateLimitOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            rateLimitOptions.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
            RateLimitPartition.GetFixedWindowLimiter(partitionKey: httpContext.User.Identity?.Name ?? httpContext.Request.Headers.Host.ToString(),
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = myOptions.AutoReplenishment,
                PermitLimit = myOptions.PermitLimit,
                Window = TimeSpan.FromSeconds(myOptions.Window),
                QueueLimit = myOptions.QueueLimit,

            }));

        });
        //ratelimiting FINAL

        builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
        builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
        builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        builder.Services.AddAutoMapper(typeof(ProdutoDTOMappingProfile));
        builder.Services.AddScoped<ITokenService, TokenService>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.ConfigureExceptionHandler();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseRateLimiter();
        app.UseCors();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}