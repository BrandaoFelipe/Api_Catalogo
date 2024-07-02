using APICatalogo.Context;
using APICatalogo.Models;
using APICatalogo.Pagination;
using X.PagedList;



namespace APICatalogo.Repositories
{
    public class CategoriaRepository : Repository<Categoria>, ICategoriaRepository
    {
        public CategoriaRepository(AppDbContext context) : base(context)
        {

        }

        public async Task<IPagedList<Categoria>> GetCategoriasFiltroNomeAsync(CategoriaFiltroNome categoriasParams)
        {
            var categorias = await GetAllAsync();

            if (!string.IsNullOrEmpty(categoriasParams.Nome))
            {
                categorias = categorias.Where(c => c.Name.Contains(categoriasParams.Nome));
            }

            //CÓDIGO SINCRONO DE PAGINAÇÃO>>> var categoriasFiltradas = PagedList<Categoria>.ToPagedList(categorias.AsQueryable(),
            //categoriasParams.PageNumber, categoriasParams.PageSize);

            var categoriasFiltradas = await categorias.ToPagedListAsync(categoriasParams.PageNumber, categoriasParams.PageSize);

            return categoriasFiltradas;
        }

        public async Task<IPagedList<Categoria>> GetProdutosAsync(ProdutosParameters categoriaParams)
        {
            var categorias = await GetAllAsync();

            var categoriaOrdenados = categorias.OrderBy(p => p.CategoriaId).AsQueryable();

            var resultado = await categorias.ToPagedListAsync(categoriaParams.PageNumber, categoriaParams.PageSize);

            return resultado;

        }

    }
}
