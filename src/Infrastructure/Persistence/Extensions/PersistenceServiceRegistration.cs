using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Context;

namespace Persistence.Extensions
{
    public static class PersistenceServiceRegistration
    {
        // Bu projede kullanacağınız servisleri IoC mekanizmasına ekleyecek olan fonksiyondur.
        // This is the function that will add the services you will use in this project to the IoC mechanism.
        public static IServiceCollection AddPersistenceServiceRegistrationRegistration(this IServiceCollection services , IConfiguration configuration)
        {
            services.AddDbContext<ApplicaitonDbContext>(opt => opt.UseSqlServer(configuration.GetConnectionString("SqlConnection")));

            return services;
        }
    }
}
