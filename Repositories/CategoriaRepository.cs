using APICatalogo.Context;
using APICatalogo.Models;
using APICatalogo.Pagination;



namespace APICatalogo.Repositories
{
    public class CategoriaRepository : Repository<Categoria>, ICategoriaRepository
    {
        public CategoriaRepository(AppDbContext context) : base(context)
        { }

        public PagedList<Categoria> GetCategoriasFiltroNome(CategoriaFiltroNome categoriasParams)
        {
            var categorias = GetAll().AsQueryable();

            if(!string.IsNullOrEmpty(categoriasParams.Nome))
            {
                categorias = categorias.Where(c => c.Name.Contains(categoriasParams.Nome));
            }

            var categoriasFiltradas = PagedList<Categoria>.ToPagedList(categorias, categoriasParams.PageNumber, categoriasParams.PageSize);

            return categoriasFiltradas;
        }

        public PagedList<Categoria> GetProdutos(ProdutosParameters categoriaParams)
            {
                var categorias = GetAll().OrderBy(p => p.CategoriaId).AsQueryable();

                var categoriaOrdenados = PagedList<Categoria>.ToPagedList(categorias, categoriaParams.PageNumber, categoriaParams.PageSize);

                return categoriaOrdenados;

            }
        
    }
}
