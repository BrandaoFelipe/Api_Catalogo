using APICatalogo.Context;
using APICatalogo.DTO.Mappings;
using APICatalogo.Extensions;
using APICatalogo.Filters;
using APICatalogo.Models;
using APICatalogo.Repositories;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;

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

        builder.Services.AddEndpointsApiExplorer();
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

        builder.Services.AddIdentity<ApplicationUser, IdentityRole>(). //ApplicationUser(que est� herdando de IdentityUser) � a classe para representar os usu�rios,
                                                                       //IdentityRole classe para as fun��es relacionado aos perfis
            AddEntityFrameworkStores<AppDbContext>() //mecanismo para armazenar os dados relacionado a classe de context
            .AddDefaultTokenProviders(); //provedores de tokens padr�o para lidar com as opera��es de autentica��o.

        string? mySqlConnection = builder.Configuration.GetConnectionString("DefaultConnection"); //String de conex�o com o MySQL
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseMySql(mySqlConnection,
                ServerVersion.AutoDetect(mySqlConnection)));

        //INICIO C�DIGO DE AUTENTICA��O BEARER
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
        }); //FINAL CODIGO DE AUTENTICA��O BEARER


        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("SuperAdminOnly", policy => policy.RequireRole("SuperAdmin").RequireClaim("id", "master"));
            options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
            options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));
            options.AddPolicy("ExclusePolicyOnly", policy => policy.RequireAssertion(context => 
                             context.User.HasClaim(claim => 
                                                   claim.Type == "id" && claim.Value =="master") || context.User.IsInRole("SuperAdmin")));
        });
        builder.Services.AddApiVersioning(o =>
        {
            o.DefaultApiVersion = new ApiVersion(1, 0);
            o.AssumeDefaultVersionWhenUnspecified = true;
            o.ReportApiVersions = true;

        }).AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV" ;
            options.SubstituteApiVersionInUrl = true;
        });

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
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}