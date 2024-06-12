using APICatalogo.Context;
using APICatalogo.DTO;
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
        private readonly IUnitOfWork _uof;
        public CategoriasController(IUnitOfWork uof)
        {
            _uof = uof;
        }

        [HttpGet]
        public ActionResult<IEnumerable<CategoriaDTO>> Get() 
        {
          var categoria = _uof.CategoriaRepository.GetAll();
            if(categoria is null)
            {
                return NotFound("Não existem dados a serem exibidos!");
            }

            var categoriaDto = new List<CategoriaDTO>();
            foreach(var categorias in categoria)
            {
                var categoriasDto = new CategoriaDTO()
                {
                    CategoriaId = categorias.CategoriaId,
                    Name = categorias.Name,
                    ImagemUrl = categorias.ImagemUrl
                };
                categoriaDto.Add(categoriasDto);
            }

            return Ok(categoriaDto);
        }
        
        [HttpGet("{id:int}", Name = "ObterCategoria")]
        public ActionResult<CategoriaDTO> Get(int id)
        {
            var categoria = _uof.CategoriaRepository.Get(c  => c.CategoriaId == id);
            if(categoria is null)
            {
                return NotFound($"Id {id} não encontrado");
            }

            var categoriaDto = new CategoriaDTO()
            {
                CategoriaId = categoria.CategoriaId,
                Name = categoria.Name,
                ImagemUrl = categoria.ImagemUrl
            };

            return Ok(categoriaDto);
        }

        [HttpPost]
        public ActionResult<CategoriaDTO> Post(CategoriaDTO categoriaDto)
        {            
            if (categoriaDto is null)
            {
                return BadRequest("Dados inválidos");
            }

            var categoria = new Categoria()
            {
                CategoriaId = categoriaDto.CategoriaId,
                Name = categoriaDto.Name,
                ImagemUrl = categoriaDto.ImagemUrl
            };

            var postCategoria = _uof.CategoriaRepository.Create(categoria);
            _uof.Commit();

            var categoriaDtoNova = new CategoriaDTO()
            {
                CategoriaId = categoria.CategoriaId,
                Name = categoria.Name,
                ImagemUrl = categoria.ImagemUrl
            };

            return new CreatedAtRouteResult("Obter categoria", new { id = categoriaDtoNova.CategoriaId }, categoriaDtoNova);
        }

        [HttpPut("{id:int}")]
        public ActionResult<CategoriaDTO> Put(int id, CategoriaDTO categoriaDto)
        {   //Para atualizar preciso comparar o id que eu informar com o id
            //da categoria, pra ver se são iguais, se não forem os dados serão inválidos
            //em seguida utilizamos o método de update e retornamos um codigo 200.
            if(id != categoriaDto.CategoriaId)
            {
                return BadRequest("Dados inválidos");
            }
            var categoria = new Categoria()
            {
                CategoriaId = categoriaDto.CategoriaId,
                Name = categoriaDto.Name,
                ImagemUrl = categoriaDto.ImagemUrl
            };

            _uof.CategoriaRepository.Update(categoria);
            _uof.Commit();
            var categoriaDtoNova = new CategoriaDTO()
            {
                CategoriaId = categoria.CategoriaId,
                Name = categoria.Name,
                ImagemUrl = categoria.ImagemUrl
            };
            return Ok(categoriaDtoNova);
        }

        [HttpDelete("{id:int}")]
        public ActionResult<CategoriaDTO> Delete(int id)
        {
            var categoria = _uof.CategoriaRepository.Get(c => c.CategoriaId == id );
            if(categoria is null)
            {
                return NotFound($"Id {id} não encontrado");
            }
            categoria = _uof.CategoriaRepository.Delete(categoria);
            _uof.Commit();
            var categoriaDtoNova = new CategoriaDTO()
            {
                CategoriaId = categoria.CategoriaId,
                Name = categoria.Name,
                ImagemUrl = categoria.ImagemUrl
            };
            return Ok(categoriaDtoNova);
        }


    }
}
