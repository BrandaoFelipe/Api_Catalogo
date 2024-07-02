using APICatalogo.Context;

namespace APICatalogo.Repositories
{
    public class UnitOfWork : IUnitOfWork // responsável pela persistencia de dados
    {
        private IProdutoRepository _produtoRepo;

        private ICategoriaRepository _categoriaRepo;
        public AppDbContext _context;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }
        public IProdutoRepository ProdutoRepository //LazyLoading significa adiar a obtenção dos objetos até que eles sejam realmente necessários
        {
            get
            {
                return _produtoRepo = _produtoRepo ?? new ProdutoRepository(_context); //verifica se existe uma instancia de _PRODUTOREPO
                                                                                       //e cria uma instancia nova caso não exista
            }
        }
        public ICategoriaRepository CategoriaRepository
        {
            get
            {
                return _categoriaRepo = _categoriaRepo ?? new CategoriaRepository(_context);
            }
        }
        public async Task CommitAsync() 
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose() //DBCONTEXT aloca recursos não gerenciáveis, o DISPOSE libera os recursos.
        {
            _context.Dispose();
        }
    }
}
