using Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions.Repository.Common
{
    public interface IWriteRepository<TEntity> where TEntity : class, IEntity, new()
    {
        Task<bool> AddAsync(TEntity entity);
        Task<bool> AddRangeAsync(IEnumerable<TEntity> entities);
        bool Update(TEntity entity);
        bool Remove(TEntity entity);
        bool RemoveRange(IEnumerable<TEntity> entities);
        Task<int> SaveChangesAsync();
    }
}
