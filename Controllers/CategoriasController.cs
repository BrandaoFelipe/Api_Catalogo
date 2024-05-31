using APICatalogo.Context;
using APICatalogo.Models;
using APICatalogo.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace APICatalogo.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        private readonly ICategoriaRepository _repository;
        public CategoriasController(ICategoriaRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Categoria>> Get() //Usando o ActionResult posso retornar código http também
        {
            var categorias = _repository.GetCategorias();
            return Ok(categorias); //cria um OkObjectResult objeto que produz uma resposta StatusCode.200OK
        }

        
        [HttpGet("{id:int}", Name = "ObterCategoria")]
        public ActionResult<Categoria> Get(int id)
        {
            var categoria = _repository.GetCategoria(id);
            if(categoria is null)
            {
                return NotFound("Categoria not found");
            }
            return Ok(categoria);
        }

        [HttpPost]
        public ActionResult<Categoria> Post(Categoria categoria)
        {
            _repository.Create(categoria);
            return new CreatedAtRouteResult("Obter categoria", new{id = categoria.CategoriaId}, categoria);
        }

        [HttpPut("{id:int}")]
        public ActionResult Put(int id, Categoria categoria)
        {
            
            if(id != categoria.CategoriaId)
            {
                return BadRequest("Invalid entry");
            }
            _repository.Update(categoria);
            return Ok(categoria);
        }

        [HttpDelete("{id:int}")]
        public ActionResult Delete(int id)
        {
            var categoriaDelete = _repository.GetCategoria(id);
            if(categoriaDelete is null)
            {
                return NotFound($"Categoria com id {id} não encontrada");
            }

            var categoria = _repository.Delete(id);
            return Ok(categoria);
        }


    }
}
