using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Persistance.Common;

public static class ConfigureServices
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddTransient(_ => new ManejadorDocumentosCfdiDbContext(configuration.GetConnectionString("DefaultConnection")));
        return serviceCollection;
    }
}
