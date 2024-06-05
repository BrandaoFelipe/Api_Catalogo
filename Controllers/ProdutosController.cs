using APICatalogo.Models;
using APICatalogo.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace APICatalogo.Controllers
{
    [Route("[controller]")] 
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        private readonly IProdutoRepository _repository;
        
        public ProdutosController(IProdutoRepository repository)
        {
            _repository = repository;                        
        }

        [HttpGet("produtos/{id}")]
        public ActionResult<IEnumerable<Produto>> GetProdutosPorCategoria(int id)
        {
            var produto = _repository.GetProdutoPorCategoria(id);
            if(produto is null)
            {
                return NotFound($"Id {id} não encontrado");
            }
            return Ok(produto);
        }

        [HttpGet]
        public ActionResult<IEnumerable<Produto>> Get()
        {
           var produtos = _repository.GetAll();
            return Ok(produtos);
        }
     
        [HttpGet("{id:int:min(1)}", Name = "ObterProduto")] 
        public ActionResult<Produto> Get(int id)
        {
            var produto = _repository.Get(p=> p.ProdutoId == id);
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
            var postProduto = _repository.Create(produto);
            return Ok(postProduto);
        }

        [HttpPut("{id:int}")] 
        public ActionResult Put(int id, Produto produto)
        {
            if(id != produto.ProdutoId)
            {
                return BadRequest($"id {id} não encontrado");
            }
            var alterProd = _repository.Update(produto);
            return Ok(alterProd);
        }

        [HttpDelete("{id:int}")]
        public ActionResult Delete(int id)
        {
            var produto = _repository.Get(p => p.ProdutoId == id);
            if(produto is null)
            {
                return NotFound("dados inválidos");
            }
            produto = _repository.Delete(produto);
            return Ok(produto);
        }
    }
}
