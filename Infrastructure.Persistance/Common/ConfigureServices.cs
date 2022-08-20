using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Persistance.Common
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddPersistenceServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<ManejadorDocumentosCfdiDbContext>();
            return serviceCollection;
        }
    }
}
