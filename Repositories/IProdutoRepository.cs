using APICatalogo.Models;

namespace APICatalogo.Repositories
{
    public interface IProdutoRepository
    {
        IEnumerable<Produto> GetProdutos();
        Produto GetProduto(int id);
        Produto Create(Produto produto);
        Produto Update(Produto produto);
        Produto Delete(int id);
    }
}
