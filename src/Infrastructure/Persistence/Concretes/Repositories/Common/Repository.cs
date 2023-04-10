using Application.Abstractions.Repositories.Common;
using Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Persistence.Concretes.Repositories.Common
{
    public class Repository<TEntity, TContext> : IRepository<TEntity> where TEntity : class, IEntity, new() where TContext : DbContext
    {
        private readonly TContext _context;

        public Repository(TContext context)
        {
            _context = context;
        }
        private DbSet<TEntity> Table => _context.Set<TEntity>();
        public IQueryable<TEntity> Query => Table.AsQueryable();

        #region Read-Metods
        public async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, bool enableTracking = true, CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> queryable = Query;
            if (!enableTracking) queryable = Query.AsNoTracking();
            if (include != null) queryable = include(queryable);
            return await queryable.FirstOrDefaultAsync(predicate, cancellationToken);
        }

        public async Task<IQueryable<TEntity>> GetListAsync(Expression<Func<TEntity, bool>>? predicate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, bool enableTracking = true, CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> queryable = Query;
            if (!enableTracking) queryable = queryable.AsNoTracking();
            if (include != null) queryable = include(queryable);
            if (predicate != null) queryable = queryable.Where(predicate);
            if (orderBy != null)
                return await Task.FromResult(orderBy(queryable));
            return await Task.FromResult(queryable);
        }

        #endregion

        #region Write-Metods
        public async Task AddAsync(TEntity entity)
        {
            await Table.AddAsync(entity);
        }

        public async Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            await Table.AddRangeAsync(entities);
        }

        public Task Remove(TEntity entity)
        {
            Table.Remove(entity);
            return Task.CompletedTask;
        }

        public Task RemoveRange(IEnumerable<TEntity> entities)
        {
            Table.RemoveRange(entities);
            return Task.CompletedTask;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public Task Update(TEntity entity)
        {
            Table.Update(entity);
            return Task.CompletedTask;
        }

        public Task UpdateRange(IEnumerable<TEntity> entities)
        {
            Table.UpdateRange(entities);
            return Task.CompletedTask;
        }
        #endregion

    }
}
