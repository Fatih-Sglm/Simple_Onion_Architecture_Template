using Application.Abstractions.Repository.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Context;
using System.Reflection;

namespace Persistence.Extensions
{
    public static class PersistenceServiceRegistration
    {
        // Bu projede kullanacağınız servisleri IoC mekanizmasına ekleyecek olan fonksiyondur.
        // This is the function that will add the services you will use in this project to the IoC mechanism.
        public static IServiceCollection AddPersistenceServiceRegistration(this IServiceCollection services , IConfiguration configuration)
        {
            services.AddDbContext<ApplicaitonDbContext>(opt => opt.UseSqlServer(configuration.GetConnectionString("SqlConnection")));
            AddRepositoryToIoC(services , Assembly.GetExecutingAssembly());
            return services;
        }

        // Repository lerin otomatik olarak IoC Container a eklenmesini sağlayan metod
        //Method that enables automatic addition of repositories to IoC Container
        private static IServiceCollection AddRepositoryToIoC(IServiceCollection services , Assembly assembly)
        {
            var reposiories = assembly.GetTypes().Where(x=> x.IsAssignableToGenericType(typeof(IRepository<>)) && !x.IsGenericType);
            foreach (var item in reposiories)
            {
                var @interface = item.GetInterfaces().FirstOrDefault(x => !x.IsGenericType) ?? throw new ArgumentNullException();
                services.AddScoped(@interface, item);
            }
            return services;
        }


        //Type in verilen generic türden türeyip türemediğini kontrol eden fonksiyon
        //Function that checks whether a given type is implementing a generic interface
        private static bool IsAssignableToGenericType(this Type givenType, Type genericType)
        {
            return givenType.GetInterfaces().Any(t => t.IsGenericType && t.GetGenericTypeDefinition() == genericType) ||
                   givenType.BaseType != null && (givenType.BaseType.IsGenericType && givenType.BaseType.GetGenericTypeDefinition() == genericType ||
                                                  givenType.BaseType.IsAssignableToGenericType(genericType));
        }
    }
}
