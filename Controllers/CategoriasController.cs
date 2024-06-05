using APICatalogo.Context;
using APICatalogo.Models;
using APICatalogo.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace APICatalogo.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        private readonly IRepository<Categoria> _repository;
        public CategoriasController(IRepository<Categoria> repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Categoria>> Get() 
        {
          var categoria = _repository.GetAll();
            return Ok(categoria);
        }
        
        [HttpGet("{id:int}", Name = "ObterCategoria")]
        public ActionResult<Categoria> Get(int id)
        {
            var categoria = _repository.Get(c  => c.CategoriaId == id);
            if(categoria is null)
            {
                return NotFound($"Id {id} não encontrado");
            }

            return Ok(categoria);
        }

        [HttpPost]
        public ActionResult<Categoria> Post(Categoria categoria)
        {            
            if (categoria is null)
            {
                return BadRequest("Dados inválidos");
            }
            var postCategoria = _repository.Create(categoria);

            return new CreatedAtRouteResult("Obter categoria", new { id = postCategoria.CategoriaId }, postCategoria);
        }

        [HttpPut("{id:int}")]
        public ActionResult Put(int id, Categoria categoria)
        {   //Para atualizar preciso comparar o id que eu informar com o id
            //da categoria, pra ver se são iguais, se não forem os dados serão inválidos
            //em seguida utilizamos o método de update e retornamos um codigo 200.
            if(id != categoria.CategoriaId)
            {
                return BadRequest("Dados inválidos");
            }
            _repository.Update(categoria);
            return Ok(categoria);
        }

        [HttpDelete("{id:int}")]
        public ActionResult Delete(int id)
        {
            var categoria = _repository.Get(c => c.CategoriaId == id );
            if(categoria is null)
            {
                return NotFound($"Id {id} não encontrado");
            }
            categoria = _repository.Delete(categoria);
            return Ok(categoria);
        }


    }
}
