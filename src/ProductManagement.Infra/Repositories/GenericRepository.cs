using Microsoft.EntityFrameworkCore;
using ProductManagement.Core.Entities;
using ProductManagement.Core.Interfaces;
using System.Linq.Expressions;

namespace ProductManagement.Infra.Repositories
{
    /// <summary>
    /// The generic repository plays a crucial role in abstracting and centralizing 
    /// data access logic. It provides a reusable and consistent way to 
    /// perform common data operations across different entities.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TContext"></typeparam>
    public class GenericRepository<TEntity, TContext> : IGenericRepository<TEntity>, IDisposable
        where TEntity : BaseEntity
        where TContext : DbContext
    {
        protected readonly TContext _context;
        private readonly DbSet<TEntity> _dbSet;
        private bool _disposed = false;

        public GenericRepository(TContext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<TEntity?> GetByIdAsync<TKey>(TKey id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(TEntity entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(TEntity entity)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
