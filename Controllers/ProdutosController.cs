using APICatalogo.Context;
using APICatalogo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Controllers
{
    [Route("[controller]")] //Cria a rota com o nome do controlador (/Produtos)
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProdutosController(AppDbContext context) //Injeção de dependência da classe AppDbContext para o controller
        {
            _context = context;
        }

        //[HttpGet("/primeiro")] // Ignora "/produtos" e utiliza apenas o especificado
        [HttpGet("{valor:alpha}")] //[HttpGet("{valor:alpha:lenght(5)}")]<minimo - [HttpGet("{valor:alpha:maxlenght(5)}")]
        public async Task<ActionResult<Produto>> Get2Async() //EndPoints
        {
            return await _context.Produtos.FirstOrDefaultAsync();
        }

        // /Produtos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Produto>>> GetAsync() //EndPoints
        {            
                var produtos = await _context.Produtos.AsNoTracking().ToListAsync();
                if (produtos is null)
                {
                    return NotFound("Produto não encontrado");
                }

                return produtos;           
        }

        //Produtos/id
        [HttpGet("{id:int:min(1)}", Name = "ObterProduto")] //restrição de rota (id:int:min(n) adiciona uma restrição
                                                            //para a requisição não chegar no metodo action
        public ActionResult<Produto> Get(int id)
        {
                var produto = _context.Produtos.AsNoTracking().FirstOrDefault(p => p.ProdutoId == id); //.AsNoTracking() Otimizar GET
                if (produto is null)
                {
                    return NotFound("Item não encontrado");
                }
                return produto;            
        }
        [HttpPost] //cria novo item
        public ActionResult Post(Produto produto)
        {
                if (produto is null)

                    return BadRequest();

                _context.Produtos.Add(produto);
                _context.SaveChanges();

                return new CreatedAtRouteResult("ObterProduto",
                    new { id = produto.ProdutoId }, produto);            
        }

        [HttpPut("{id:int}")] //Modifica o item //Route que utiliza o Id no url
        public ActionResult Put(int id, Produto produto)
        {
                if (id != produto.ProdutoId)
                {
                    return BadRequest();
                }
                _context.Entry(produto).State = EntityState.Modified;
                _context.SaveChanges();
                return Ok("Done!");            
        }

        [HttpDelete("{id:int}")]
        public ActionResult Delete(int id)
        {
                var produto = _context.Produtos.FirstOrDefault(p => p.ProdutoId == id);
                if (produto is null)
                {
                    return NotFound("Item não encontrado");
                }

                _context.Produtos.Remove(produto);
                _context.SaveChanges();

                return Ok(produto);            
        }
    }
}
