using Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Context
{
    public class ApplicaitonDbContext : DbContext
    {
        public ApplicaitonDbContext(DbContextOptions options) : base(options)
        {
        }

        // Entityleri db ye kaydederken içerdiği dateTime propertylerini otomatik olarak ekleyen bir metod
        // Not : Eğer PostgreSql kullanacaksanız DateTime yerine DateTime.Utc.Now Kullanmanız gerekiyor

        // A method that automatically adds dateTime properties contained in entities while saving to the database
        // Note: If you're using PostgreSql, you need to use DateTime.Utc.Now instead of DateTime
        public async override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var datas = ChangeTracker.Entries<IEntity>();
            foreach (var data in datas)
            {
                _ = data.State switch
                {
                    EntityState.Added => data.Entity.CreatedDate = DateTime.Now,
                    EntityState.Modified => data.Entity.ModifiedDate = DateTime.Now,
                    _ => DateTime.Now
                };
            }
            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
