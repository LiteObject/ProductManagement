using ProductManagement.Core.Entities;
using System.Linq.Expressions;

namespace ProductManagement.Core.Interfaces
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        IEnumerable<T> GetAll();
        T? GetById<TKey>(TKey id);
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
        IEnumerable<T> Find(Expression<Func<T, bool>> predicate);
    }
}
