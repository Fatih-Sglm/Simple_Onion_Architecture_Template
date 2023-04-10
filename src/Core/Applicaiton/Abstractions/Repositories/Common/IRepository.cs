using Domain.Entities.Common;

namespace Application.Abstractions.Repositories.Common
{
    public interface IRepository<TEntity> : IWriteRepository<TEntity>, IReadRepository<TEntity> where TEntity : class, IEntity, new()
    {
        IQueryable<TEntity> Query { get; }
    }
}
