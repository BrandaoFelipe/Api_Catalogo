using APICatalogo.DTO;
using APICatalogo.Models;
using APICatalogo.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace APICatalogo.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        private readonly IUnitOfWork _uof;
        private readonly IMapper _mapper;

        public ProdutosController(IUnitOfWork uof, IMapper mapper)
        {
            _uof = uof;
            _mapper = mapper;
        }

        [HttpPatch("{id}/PartialUpdate")]
        public ActionResult<ProdutoDTOResponseRequest> Patch(int id, JsonPatchDocument<ProdutoDTOUpdateRequest> patchProdutoDTO)
        {
            if(patchProdutoDTO is null || id <= 0)
            {
                return BadRequest();
            }

            var produto = _uof.ProdutoRepository.Get(p => p.ProdutoId == id);
            if(produto is null)
            {
                return NotFound();
            }

            var produtoUpdateRequest = _mapper.Map<ProdutoDTOUpdateRequest>(produto);

            patchProdutoDTO.ApplyTo(produtoUpdateRequest, ModelState);

            if(!ModelState.IsValid || TryValidateModel(produtoUpdateRequest))
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(produtoUpdateRequest, produto);
            //Quando você faz "_mapper.Map(produtoUpdateRequest, produto)",
            //o AutoMapper olha para o tipo de "produto" e o tipo de "produtoUpdateRequest"
            //e faz uma inferência de que você quer mapear de "produtoUpdateRequest" para "produto".

            _uof.ProdutoRepository.Update(produto);

            _uof.Commit();

            return Ok(_mapper.Map<ProdutoDTOResponseRequest>(produto));
        }


        [HttpGet("produtos/{id}")]
        public ActionResult<IEnumerable<ProdutoDTO>> GetProdutosPorCategoria(int id)
        {
            var produto = _uof.ProdutoRepository.GetProdutoPorCategoria(id);
            if (produto is null)
            {
                return NotFound($"Id {id} não encontrado");
            }
            //destino = _mapper.Map<Destino>(origem);
            var produtoDto = _mapper.Map<IEnumerable<ProdutoDTO>>(produto);

            return Ok(produtoDto);
        }

        [HttpGet]
        public ActionResult<IEnumerable<ProdutoDTO>> Get()
        {
            var produtos = _uof.ProdutoRepository.GetAll();

            if (produtos is null)
            {
                return BadRequest();
            }

            var produtosDto = _mapper.Map<IEnumerable<ProdutoDTO>>(produtos);

            return Ok(produtosDto);
        }

        [HttpGet("{id:int:min(1)}", Name = "ObterProduto")]
        public ActionResult<ProdutoDTO> Get(int id)
        {
            var produto = _uof.ProdutoRepository.Get(p => p.ProdutoId == id);
            if (produto is null)
            {
                return NotFound("Id não encontrado");
            }

            var produtoDto = _mapper.Map<ProdutoDTO>(produto);

            return Ok(produtoDto);
        }
        [HttpPost]
        public ActionResult<ProdutoDTO> Post(ProdutoDTO produtoDto)
        {
            if (produtoDto is null)
            {
                return BadRequest("Dados inválidos");
            }

            var postProduto = _mapper.Map<Produto>(produtoDto);

            _uof.ProdutoRepository.Create(postProduto);

            _uof.Commit();

            var produtoDtoNovo = _mapper.Map<ProdutoDTO>(postProduto);

            return Ok(produtoDtoNovo);
        }

        [HttpPut("{id:int}")]
        public ActionResult<ProdutoDTO> Put(int id, ProdutoDTO produtoDto)
        {
            if (id != produtoDto.ProdutoId)
            {
                return BadRequest($"id {id} não encontrado");
            }
            var alterProd = _mapper.Map<Produto>(produtoDto);

            _uof.ProdutoRepository.Update(alterProd);

            _uof.Commit();

            var produtoDtoNovo = _mapper.Map<ProdutoDTO>(alterProd);

            return Ok(produtoDtoNovo);
        }

        [HttpDelete("{id:int}")]
        public ActionResult<ProdutoDTO> Delete(int id)
        {
            var produto = _uof.ProdutoRepository.Get(p => p.ProdutoId == id);

            if (produto is null)
            {
                return NotFound("dados inválidos");
            }

            _uof.ProdutoRepository.Delete(produto);

            var produtoDto = _mapper.Map<ProdutoDTO>(produto);

            _uof.Commit();

            return Ok(produto);
        }
    }
}