using APICatalogo.Models;
using APICatalogo.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace APICatalogo.Controllers
{
    [Route("[controller]")] 
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        private readonly IUnitOfWork _uof;
        
        public ProdutosController(IUnitOfWork uof)
        {
            _uof = uof;                        
        }

        [HttpGet("produtos/{id}")]
        public ActionResult<IEnumerable<Produto>> GetProdutosPorCategoria(int id)
        {
            var produto = _uof.ProdutoRepository.GetProdutoPorCategoria(id);
            if(produto is null)
            {
                return NotFound($"Id {id} não encontrado");
            }
            return Ok(produto);
        }

        [HttpGet]
        public ActionResult<IEnumerable<Produto>> Get()
        {
           var produtos = _uof.ProdutoRepository.GetAll();
            return Ok(produtos);
        }
     
        [HttpGet("{id:int:min(1)}", Name = "ObterProduto")] 
        public ActionResult<Produto> Get(int id)
        {
            var produto = _uof.ProdutoRepository.Get(p=> p.ProdutoId == id);
            if(produto is null)
            {
                return NotFound("Id não encontrado");
            }
            return Ok(produto);
        }
        [HttpPost]
        public ActionResult Post(Produto produto)
        {
           if(produto is null)
            {
                return BadRequest("Dados inválidos");
            }
            var postProduto = _uof.ProdutoRepository.Create(produto);
            _uof.Commit();
            return Ok(postProduto);
        }

        [HttpPut("{id:int}")] 
        public ActionResult Put(int id, Produto produto)
        {
            if(id != produto.ProdutoId)
            {
                return BadRequest($"id {id} não encontrado");
            }
            var alterProd = _uof.ProdutoRepository.Update(produto);
            _uof.Commit();
            return Ok(alterProd);
        }

        [HttpDelete("{id:int}")]
        public ActionResult Delete(int id)
        {
            var produto = _uof.ProdutoRepository.Get(p => p.ProdutoId == id);
            if(produto is null)
            {
                return NotFound("dados inválidos");
            }
            produto = _uof.ProdutoRepository.Delete(produto);
            _uof.Commit();
            return Ok(produto);
        }
    }
}
