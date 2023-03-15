using System.Reflection;
using ARSoftware.Cfdi.DescargaMasiva;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Application.Common;

public static class ConfigureServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(configuration => configuration.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        services.AddSingleton<ConfiguracionAplicacion>();

        services.AddCfdiDescargaMasivaServices();

        return services;
    }
}
