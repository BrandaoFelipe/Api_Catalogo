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

        [HttpGet]
        public ActionResult<IEnumerable<Produto>> Get()
        {
            var produto = _repository.GetProdutos();
            
            return Ok(produto);
        }
     
        [HttpGet("{id:int:min(1)}", Name = "ObterProduto")] 
        public ActionResult<Produto> Get(int id)
        {
            var produto = _repository.GetProduto(id);
            if (produto is null)
            {
                return BadRequest($"Id {id} Not found!");
            }
            return produto;
        }
        [HttpPost]
        public ActionResult Post(Produto produto)
        {
            _repository.Create(produto);
            return new CreatedAtRouteResult("ObterProduto", new { id = produto.ProdutoId }, produto);
        }

        [HttpPut("{id:int}")] 
        public ActionResult Put(int id, Produto produto)
        {
            var getProduto = _repository.GetProduto(id);
            if (getProduto.ProdutoId != id)
            {
                return BadRequest("Id not found!");
            }
            _repository.Update(produto);
            return Ok(produto);
        }

        [HttpDelete("{id:int}")]
        public ActionResult Delete(int id)
        {
            var produto = _repository.GetProduto(id);
            if (produto.ProdutoId != id)
            {
                BadRequest($"Id{id} not found!");
            }

            _repository.Delete(id);
            return Ok(produto);
        }
    }
}
