using APICatalogo.Models;
using APICatalogo.Pagination;

namespace APICatalogo.Repositories
{
    public interface ICategoriaRepository : IRepository<Categoria>
    {
        PagedList<Categoria> GetProdutos(ProdutosParameters produtoParams);
        PagedList<Categoria> GetCategoriasFiltroNome(CategoriaFiltroNome categoriasParams);
    }
}
