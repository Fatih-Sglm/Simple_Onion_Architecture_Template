using Domain.Entities.Common;

namespace Application.Abstractions.Repositories.Common
{
    public interface IWriteRepository<TEntity> where TEntity : class, IEntity, new()
    {
        Task AddAsync(TEntity entity);
        Task AddRangeAsync(IEnumerable<TEntity> entities);
        Task Update(TEntity entity);
        Task UpdateRange(IEnumerable<TEntity> entities);
        Task Remove(TEntity entity);
        Task RemoveRange(IEnumerable<TEntity> entities);
        Task<int> SaveChangesAsync();
    }
}
