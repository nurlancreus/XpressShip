using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using XpressShip.Application.Abstractions.Repositories;
using XpressShip.Domain.Entities.Base;

namespace XpressShip.Infrastructure.Persistence.Repositories
{
    public class Repository<T>(AppDbContext dbContext) : IRepository<T> where T : BaseEntity
    {
        private readonly DbSet<T> _dbSet = dbContext.Set<T>();

        public DbSet<T> Table => _dbSet;
        public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddAsync(entity, cancellationToken);
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, bool isTrackingActive = true, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.Where(predicate);

            if (!isTrackingActive)
            {
                query = query.AsNoTracking();
            }

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<T>> GetAllAsync(bool isTrackingActive = true, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.AsQueryable();

            if (!isTrackingActive)
            {
                query = query.AsNoTracking();
            }

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<T?> GetByIdAsync(Guid id, bool isTrackingActive = true, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.AsQueryable();

            if (!isTrackingActive)
            {
                query = query.AsNoTracking();
            }

            return await query.FirstAsync(x => x.Id == id, cancellationToken);
        }

        public Task<bool> IsExistAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return _dbSet.AnyAsync(predicate, cancellationToken);
        }
    }
}
