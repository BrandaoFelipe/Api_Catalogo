using APICatalogo.Context;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly AppDbContext _context;
        public Repository(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<T> GetAll()
        {
            return [.. _context.Set<T>().AsNoTracking()]; //Set do Entity framework core é usado para acessar uma coleção ou uma tabela.
                                                          //ASNOTRAKING é usado apenas nos métodos de realizar consulta.
                                                          //método de utilizar TOLIST() [.. ]
        }
        public T? Get(Expression<Func<T, bool>> predicate)
        {
            return _context.Set<T>().AsNoTracking().FirstOrDefault(predicate); 
        }
        public T Create(T entity)
        {
            _context.Set<T>().Add(entity);
            //_context.SaveChanges(); UNITOFWORK agora é responsável por enviar os dados para o DB
            return entity;
        }
        public T Update(T entity)
        {
            //_context.Set<T>().Entry(entity).State = EntityState.Modified;
            _context.Set<T>().Update(entity);
            //_context.SaveChanges(); UNITOFWORK agora é responsável por enviar os dados para o DB
            return entity;
            
        }
        public T Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
            //_context.SaveChanges(); UNITOFWORK agora é responsável por enviar os dados para o DB
            return entity;
        }
    }
}
