using APICatalogo.DTO;
using APICatalogo.Models;
using APICatalogo.Pagination;
using APICatalogo.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security;
using X.PagedList;

namespace APICatalogo.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = false)]
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
        public async Task<ActionResult<ProdutoDTOResponseRequest>> Patch(int id, JsonPatchDocument<ProdutoDTOUpdateRequest> patchProdutoDTO)
        {
            if(patchProdutoDTO is null || id <= 0)
            {
                return BadRequest();
            }

            var produto = await _uof.ProdutoRepository.GetAsync(p => p.ProdutoId == id);
            if(produto is null)
            {
                return NotFound();
            }           

            var produtoUpdateRequest = _mapper.Map<ProdutoDTOUpdateRequest>(produto);

            patchProdutoDTO.ApplyTo(produtoUpdateRequest, ModelState);

            if (!ModelState.IsValid && TryValidateModel(produtoUpdateRequest))//TryValidateModel não funcionando xD
            {
                return BadRequest(ModelState);
            }
            if(produtoUpdateRequest.DataCadastro <= DateTime.Now.Date)
            {
                return BadRequest("A data deve ser maior que a atual");
            }

            _mapper.Map(produtoUpdateRequest, produto); //AutoMapper atualiza o "produto" com as atualizações do "produtoUpdateRequest"
                                                        //sem alterar o mapeamento           
            _uof.ProdutoRepository.Update(produto);

            await _uof.CommitAsync();

            return Ok(_mapper.Map<ProdutoDTOResponseRequest>(produto));
        }


        [HttpGet("produtos/{id}")]
        public async Task<ActionResult<IEnumerable<ProdutoDTO>>> GetProdutosPorCategoria(int id)
        {
            var produto = await _uof.ProdutoRepository.GetProdutoPorCategoriaAsync(id);
            if (produto is null)
            {
                return NotFound($"Id {id} não encontrado");
            }
            //destino = _mapper.Map<Destino>(origem);
            var produtoDto = _mapper.Map<IEnumerable<ProdutoDTO>>(produto);

            return Ok(produtoDto);
        }

        [HttpGet("filter/preco/pagination")]
        public async Task<ActionResult<IEnumerable<ProdutoDTO>>> GetProdutosFilterPreco([FromQuery] ProdutosFiltroPreco produtosFiltroParam)
        {
            var produtos = await _uof.ProdutoRepository.GetProdutosFiltroPrecoAsync(produtosFiltroParam);

            var produtosDto = _mapper.Map<IEnumerable<ProdutoDTO>>(produtos);

            ObterProdutos(produtos);

            return Ok(produtosDto);
        }

        
        [HttpGet("pagination")]
        public async Task<ActionResult<IEnumerable<ProdutoDTO>>> Get([FromQuery] ProdutosParameters produtoParams)
        {
            var produtos = await _uof.ProdutoRepository.GetProdutosAsync(produtoParams);

            ObterProdutos(produtos);
            
            var produtosDto = _mapper.Map<IEnumerable<ProdutoDTO>>(produtos);

            return Ok(produtosDto);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProdutoDTO>>> Get()
        {
            var produtos = await _uof.ProdutoRepository.GetAllAsync();

            if (produtos is null)
            {
                return BadRequest();
            }

            var produtosDto = _mapper.Map<IEnumerable<ProdutoDTO>>(produtos);

            return Ok(produtosDto);
        }

        [HttpGet("{id:int:min(1)}", Name = "ObterProduto")]
        public async Task <ActionResult<ProdutoDTO>> Get(int id)
        {
            var produto = await _uof.ProdutoRepository.GetAsync(p => p.ProdutoId == id);
            if (produto is null)
            {
                return NotFound("Id não encontrado");
            }

            var produtoDto = _mapper.Map<ProdutoDTO>(produto);

            return Ok(produtoDto);
        }
        [HttpPost]
        public async Task<ActionResult<ProdutoDTO>> Post(ProdutoDTO produtoDto)
        {
            if (produtoDto is null)
            {
                return BadRequest("Dados inválidos");
            }

            var postProduto = _mapper.Map<Produto>(produtoDto);

           _uof.ProdutoRepository.Create(postProduto);

            await _uof.CommitAsync();

            var produtoDtoNovo = _mapper.Map<ProdutoDTO>(postProduto);

            return Ok(produtoDtoNovo);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<ProdutoDTO>> Put(int id, ProdutoDTO produtoDto)
        {
            if (id != produtoDto.ProdutoId)
            {
                return BadRequest($"id {id} não encontrado");
            }
            var alterProd = _mapper.Map<Produto>(produtoDto);

            _uof.ProdutoRepository.Update(alterProd);

            await _uof.CommitAsync();

            var produtoDtoNovo = _mapper.Map<ProdutoDTO>(alterProd);

            return Ok(produtoDtoNovo);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ProdutoDTO>> Delete(int id)
        {
            var produto = await _uof.ProdutoRepository.GetAsync(p => p.ProdutoId == id);

            if (produto is null)
            {
                return NotFound("dados inválidos");
            }

            _uof.ProdutoRepository.Delete(produto);

            var produtoDto = _mapper.Map<ProdutoDTO>(produto);

            await _uof.CommitAsync();

            return Ok(produto);
        }
        private void ObterProdutos(IPagedList<Produto> produtos)
        {
            var metadata = new
            {
                produtos.Count,
                produtos.PageSize,
                produtos.PageCount,
                produtos.TotalItemCount,
                produtos.HasNextPage,
                produtos.HasPreviousPage
            };

            Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));
        }

    }
}