using APICatalogo.Models;

namespace APICatalogo.Repositories
{
    public interface IUnitOfWork
    {
        //implementação de métodos genéricos - com a implementação de métodos genéricos perdemos a flexibilidade de
        //implementar métodos personalizados.
        //IRepository<Produto> ProdutoRepository {  get; }
        //IRepository<Categoria> CategoriaRepository { get; }
        

        //implemetanção de propriedades específicas
        IProdutoRepository ProdutoRepository { get; }
        ICategoriaRepository CategoriaRepository { get; }

        void Commit(); 
    }
}
