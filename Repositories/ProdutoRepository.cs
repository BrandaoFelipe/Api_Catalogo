using APICatalogo.Context;
using APICatalogo.Models;
using APICatalogo.Pagination;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace APICatalogo.Repositories
{
    public class ProdutoRepository : Repository<Produto>, IProdutoRepository
    {
        public ProdutoRepository(AppDbContext context) : base(context)
        {
        }
        public async Task<IEnumerable<Produto>> GetProdutoPorCategoriaAsync(int id)
        {
            var produtos = await GetAllAsync();

            var produtosFiltro = produtos.Where(c => c.CategoriaId == id);

            return produtosFiltro;
        }       

        public async Task<IPagedList<Produto>> GetProdutosAsync(ProdutosParameters produtosParams)
        {
            var produtos = await GetAllAsync();

            var produtosOrderBy = produtos.OrderBy(p => p.ProdutoId).AsQueryable();

            var produtosOrdenados = await produtos.ToPagedListAsync(produtosParams.PageNumber, produtosParams.PageSize);

            return produtosOrdenados;

        }

        public async Task<IPagedList<Produto>> GetProdutosFiltroPrecoAsync(ProdutosFiltroPreco produtosFiltroParams)
        {
            var produtos = await GetAllAsync();
            
            if(produtosFiltroParams.Preco.HasValue && !string.IsNullOrEmpty(produtosFiltroParams.PrecoCriterio))
            {
                if(produtosFiltroParams.PrecoCriterio.Equals("maior", StringComparison.OrdinalIgnoreCase))
                {
                    produtos = produtos.Where(p => p.Preco > produtosFiltroParams.Preco.Value).OrderBy(p => p.Preco);
                }
                else if(produtosFiltroParams.PrecoCriterio.Equals("menor", StringComparison.OrdinalIgnoreCase))
                {
                    produtos = produtos.Where(p => p.Preco < produtosFiltroParams.Preco.Value).OrderBy(p => p.Preco);
                }
                else if (produtosFiltroParams.PrecoCriterio.Equals("igual", StringComparison.OrdinalIgnoreCase))
                {
                    produtos = produtos.Where(p => p.Preco == produtosFiltroParams.Preco.Value).OrderBy(p => p.Preco);
                }                
            }

            var produtosFiltrados = await produtos.ToPagedListAsync(produtosFiltroParams.PageNumber, produtosFiltroParams.PageSize);

            return produtosFiltrados;
        }
    } 
}
