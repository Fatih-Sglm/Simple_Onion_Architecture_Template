using Domain.Entities.Common;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Application.Abstractions.Repositories.Common
{
    public interface IReadRepository<TEntity> where TEntity : class, IEntity, new()
    {
        Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>,
                                         IIncludableQueryable<TEntity, object>>? include = null, bool enableTracking = true,
                                         CancellationToken cancellationToken = default);

        Task<IQueryable<TEntity>> GetListAsync(Expression<Func<TEntity, bool>>? predicate = null,
                                        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
                                        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
                                        bool enableTracking = true,
                                        CancellationToken cancellationToken = default);
    }
}
