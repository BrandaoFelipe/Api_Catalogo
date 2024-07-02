using APICatalogo.Models;
using APICatalogo.Pagination;
using X.PagedList;

namespace APICatalogo.Repositories
{
    public interface ICategoriaRepository : IRepository<Categoria>
    {
        Task<IPagedList<Categoria>> GetProdutosAsync(ProdutosParameters produtoParams);
        Task<IPagedList<Categoria>> GetCategoriasFiltroNomeAsync(CategoriaFiltroNome categoriasParams);
    }
}
