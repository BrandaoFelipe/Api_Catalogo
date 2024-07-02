using APICatalogo.Models;
using APICatalogo.Pagination;
using X.PagedList;

namespace APICatalogo.Repositories
{
    public interface IProdutoRepository : IRepository<Produto>
    {
        Task<IEnumerable<Produto>> GetProdutoPorCategoriaAsync(int id);
        Task<IPagedList<Produto>> GetProdutosAsync(ProdutosParameters produtoParams);
        Task<IPagedList<Produto>> GetProdutosFiltroPrecoAsync(ProdutosFiltroPreco produtosFiltroParams);

    }
}
