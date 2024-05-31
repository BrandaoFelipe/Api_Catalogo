using APICatalogo.Context;
using APICatalogo.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace APICatalogo.Repositories
{
    public class CategoriaRepository : ICategoriaRepository //Repository são usados como um intermediário
                                                            //entre os controladores e o acesso ao banco de dados.
                                                            //toda a lógica de acesso aos dados é posta nessa classe
                                                            //e os controladores e a classe dbcontext ficam apenas
                                                            //com a injeção de dependencia da classe repository.
    
    {
        private readonly AppDbContext _context;

        public CategoriaRepository(AppDbContext context)
        {
            _context = context;
        }

        public Categoria Create(Categoria categoria)
        {
            if(categoria is null)
            {
                throw new ArgumentNullException(nameof(categoria));
            }
            _context.Categorias.Add(categoria);
            _context.SaveChanges();
            return categoria;
        }

        public Categoria Delete(int id)
        {
            var categoria = _context.Categorias.Find(id);//FirstOrDefault vai direto no banco de dados e
                                                         //o Find vai na memória mas utiliza apenas chaves primárias!
            
            if(categoria is null)
            {
                throw new ArgumentNullException(nameof(categoria));
            }            
            _context.Remove(categoria);
            _context.SaveChanges();
            return categoria;
        }

        public Categoria GetCategoria(int id)
        {
            /*var categoria = _context.Categorias.Find(id);
            return categoria;*/
            return _context.Categorias.FirstOrDefault(c => c.CategoriaId == id);            
        }

        public IEnumerable<Categoria> GetCategorias()
        {
            return _context.Categorias.ToList();
        }

        public Categoria Update(Categoria categoria)
        {
            if(categoria is null)
            {
                throw new ArgumentNullException(nameof(categoria));
            }            
            _context.Entry(categoria).State = EntityState.Modified;
            _context.SaveChanges();
            return categoria;
        }
    }
}
