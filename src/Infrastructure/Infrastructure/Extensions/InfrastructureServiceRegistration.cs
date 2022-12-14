using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions
{
    public static class InfrastructureServiceRegistration
    {
        // Bu projede kullanacağınız servisleri IoC mekanizmasına ekleyecek olan fonksiyondur.
        // This is the function that will add the services you will use in this project to the IoC mechanism.
        public static IServiceCollection AddInfrastructureServiceRegistration(this IServiceCollection services)
        {


            return services;
        }
    }
}
