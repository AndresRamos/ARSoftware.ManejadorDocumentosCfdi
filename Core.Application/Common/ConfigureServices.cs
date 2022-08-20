using ARSoftware.Cfdi.DescargaMasiva.Interfaces;
using ARSoftware.Cfdi.DescargaMasiva.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Application.Common
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddTransient<IHttpSoapClient, HttpSoapClient>();
            services.AddTransient<IAutenticacionService, AutenticacionService>();
            services.AddTransient<ISolicitudService, SolicitudService>();
            services.AddTransient<IVerificacionService, VerificacionService>();
            services.AddTransient<IDescargaService, DescargaService>();

            return services;
        }
    }
}
