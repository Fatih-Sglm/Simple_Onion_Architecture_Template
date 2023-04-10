using Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions.Repository.Common
{
    public interface IRepository<TEntity> : IWriteRepository<TEntity> , IReadRepository<TEntity>  where TEntity : class, IEntity, new()
    {
        IQueryable<TEntity> Query { get; }
    }
}
