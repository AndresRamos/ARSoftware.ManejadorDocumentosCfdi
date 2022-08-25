using System.Reflection;
using ARSoftware.Cfdi.DescargaMasiva;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Application.Common;

public static class ConfigureServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(Assembly.GetExecutingAssembly());

        services.AddSingleton<ConfiguracionAplicacion>();

        services.AddCfdiDescargaMasivaServices();

        return services;
    }
}
