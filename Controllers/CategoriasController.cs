using APICatalogo.Context;
using APICatalogo.DTO;
using APICatalogo.DTO.Mappings;
using APICatalogo.Models;
using APICatalogo.Pagination;
using APICatalogo.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using X.PagedList;


namespace APICatalogo.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        private readonly IUnitOfWork _uof;
        private readonly IMapper _mapper;

        public CategoriasController(IUnitOfWork uof, IMapper mapper)
        {
            _uof = uof;
            _mapper = mapper;

        }

        [HttpGet("pagination")]
        public async Task<ActionResult<IEnumerable<CategoriaDTO>>> Get([FromQuery] ProdutosParameters produtoParams)
        {
            var categorias = await _uof.CategoriaRepository.GetProdutosAsync(produtoParams);

            ObterCategoria(categorias);

            var categoriaDto = _mapper.Map<IEnumerable<CategoriaDTO>>(categorias);

            return Ok(categoriaDto);
        }

        [HttpGet("filter/nome/pagination")]
        public async Task<ActionResult<IEnumerable<CategoriaDTO>>> GetCategoriasFiltradas([FromQuery] CategoriaFiltroNome categoriaParams)
        {
            var categorias = await _uof.CategoriaRepository.GetCategoriasFiltroNomeAsync(categoriaParams);

            var categoriaDto = _mapper.Map<IEnumerable<CategoriaDTO>>(categorias);

            ObterCategoria(categorias);

            return Ok(categoriaDto);           
        }

        

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoriaDTO>>> Get() 
        {
          var categoria = await _uof.CategoriaRepository.GetAllAsync();
            if(categoria is null)
            {
                return NotFound("Não existem dados a serem exibidos!");
            }

            var categoriaDto = _mapper.Map<IEnumerable<CategoriaDTO>>(categoria);

            return Ok(categoriaDto);
        }
        
        [HttpGet("{id:int}", Name = "ObterCategoria")]
        public async Task<ActionResult<CategoriaDTO>> Get(int id)
        {
            var categoria = await _uof.CategoriaRepository.GetAsync(c  => c.CategoriaId == id);
            if(categoria is null)
            {
                return NotFound($"Id {id} não encontrado");
            }

            //var dtoCategoria = categoria.ToCategoriaDTO(); manual mapper
            var dtoCategoria = _mapper.Map<CategoriaDTO>(categoria);

            return Ok(dtoCategoria);
        }

        [HttpPost]
        public async Task<ActionResult<CategoriaDTO>> Post(CategoriaDTO categoriaDto)
        {            
            if (categoriaDto is null)
            {
                return BadRequest("Dados inválidos");
            }

            //var categoria = categoriaDto.ToCategoria(); manual mapper
            var categoria = _mapper.Map<Categoria>(categoriaDto);

            _uof.CategoriaRepository.Create(categoria);
            
           await _uof.CommitAsync();

            //var categoriaDtoNova = categoria.ToCategoriaDTO(); manual mapper
            var categoriaDtoNova = _mapper.Map<CategoriaDTO>(categoria);
            
            return Ok(categoriaDtoNova); 
            // esse método abaixo está retornando um código 500 e mesmo assim está sendo executado.
            // new CreatedAtRouteResult("Obter categoria", new { id = categoriaDtoNova.CategoriaId }, categoriaDtoNova);
            
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<CategoriaDTO>> Put(int id, CategoriaDTO categoriaDto)
        {   //Para atualizar preciso comparar o id que eu informar com o id
            //da categoria, pra ver se são iguais, se não forem os dados serão inválidos
            //em seguida utilizamos o método de update e retornamos um codigo 200.

            if(id != categoriaDto.CategoriaId)
            {
                return BadRequest("Dados inválidos");
            }
            
            var categoria = _mapper.Map<Categoria>(categoriaDto);

            _uof.CategoriaRepository.Update(categoria);

            await _uof.CommitAsync();

            var categoriaDtoNova = _mapper.Map<CategoriaDTO>(categoria);

            return Ok(categoriaDtoNova);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<CategoriaDTO>> Delete(int id)
        {
            var categoria = await _uof.CategoriaRepository.GetAsync(c => c.CategoriaId == id );

            if(categoria is null)
            {
                return NotFound($"Id {id} não encontrado");
            }

            _uof.CategoriaRepository.Delete(categoria);

            await _uof.CommitAsync();

            var categoriaDtoNova = _mapper.Map<CategoriaDTO>(categoria);

            return Ok(categoriaDtoNova);
        }

        private void ObterCategoria(IPagedList<Categoria> categorias)
        {
            var metadata = new
            {
                categorias.Count,
                categorias.PageSize,
                categorias.PageCount,
                categorias.TotalItemCount,
                categorias.HasNextPage,
                categorias.HasPreviousPage
            };

            Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));
        }

    }
}
