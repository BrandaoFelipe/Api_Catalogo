using APICatalogo.Models;

namespace APICatalogo.Repositories
{
    public interface ICategoriaRepository
    {
        //por se tratar de uma interface, estamos implementando métodos,
        //por isso não usamos {get set} e o uso do (); no final deve-se ao fato de serem métodos();
        IEnumerable<Categoria> GetCategorias();                                                 
        Categoria GetCategoria(int id);
        Categoria Create(Categoria categoria);
        Categoria Update(Categoria categoria);
        Categoria Delete(int id);




    }
}
