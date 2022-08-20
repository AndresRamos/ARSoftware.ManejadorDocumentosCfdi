using System.Reflection;
using ARSoftware.Cfdi.DescargaMasiva.Interfaces;
using ARSoftware.Cfdi.DescargaMasiva.Services;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Application.Common
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddMediatR(Assembly.GetExecutingAssembly());

            services.AddSingleton<ConfiguracionAplicacion>();

            services.AddCfdiDescargaServices();

            return services;
        }

        private static void AddCfdiDescargaServices(this IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddTransient<IHttpSoapClient, HttpSoapClient>();
            services.AddTransient<IAutenticacionService, AutenticacionService>();
            services.AddTransient<ISolicitudService, SolicitudService>();
            services.AddTransient<IVerificacionService, VerificacionService>();
            services.AddTransient<IDescargaService, DescargaService>();
        }
    }
}
