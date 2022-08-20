using Contpaqi.Sql.Comercial.Empresa.Factories;
using Contpaqi.Sql.Comercial.Generales.Factories;
using Contpaqi.Sql.Contabilidad.Empresa.Factories;
using Contpaqi.Sql.Contabilidad.Generales.Factories;
using Core.Application.Common;
using Core.Application.Comprobantes.Interfaces;
using Core.Application.Empresas.Interfaces;
using Core.Application.Rfcs.Interfaces;
using Infrastructure.Contpaqi.ADD.Factories;
using Infrastructure.Contpaqi.ADD.Repositories;
using Infrastructure.Contpaqi.Comercial.Repositories;
using Infrastructure.Contpaqi.Contabilidad.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Common
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddContpaqiComercialServices();
            serviceCollection.AddContpaqiContabilidadServices();
            serviceCollection.AddContpaqiAddServices();
            return serviceCollection;
        }

        private static void AddContpaqiComercialServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient(c =>
            {
                var configuracionAplicacion = c.GetRequiredService<ConfiguracionAplicacion>();
                return ComercialGeneralesDbContextFactory.CreateInstance(configuracionAplicacion.ConfiguracionGeneral
                    .ConfiguracionContpaqiComercial.ContpaqiSqlConnectionString);
            });

            serviceCollection.AddTransient(c =>
            {
                var configuracionAplicacion = c.GetRequiredService<ConfiguracionAplicacion>();
                return ComercialEmpresaDbContextFactory.CreateInstance(
                    configuracionAplicacion.ConfiguracionGeneral.ConfiguracionContpaqiComercial.ContpaqiSqlConnectionString,
                    configuracionAplicacion.ConfiguracionGeneral.ConfiguracionContpaqiComercial.Empresa.BaseDatos);
            });
            serviceCollection.AddTransient<IEmpresaComercialRepository, EmpresaComercialRepository>();
            serviceCollection.AddTransient<IRfcComercialRepository, RfcComercialRepository>();
        }

        private static void AddContpaqiContabilidadServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient(c =>
            {
                var configuracionAplicacion = c.GetRequiredService<ConfiguracionAplicacion>();
                return ContabilidadGeneralesDbContextFactory.CreateInstance(configuracionAplicacion.ConfiguracionGeneral
                    .ConfiguracionContpaqiContabilidad.ContpaqiSqlConnectionString);
            });

            serviceCollection.AddTransient(c =>
            {
                var configuracionAplicacion = c.GetRequiredService<ConfiguracionAplicacion>();
                return ContabilidadEmpresaDbContextFactory.CreateInstance(
                    configuracionAplicacion.ConfiguracionGeneral.ConfiguracionContpaqiContabilidad.ContpaqiSqlConnectionString,
                    configuracionAplicacion.ConfiguracionGeneral.ConfiguracionContpaqiContabilidad.Empresa.BaseDatos);
            });
            serviceCollection.AddTransient<IEmpresaContabilidadRepository, EmpresaContabilidadRepository>();
            serviceCollection.AddTransient<IRfcContabilidadRepository, RfcContabilidadRepository>();
        }

        private static void AddContpaqiAddServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient(provider =>
            {
                var configuracionAplicacion = provider.GetRequiredService<ConfiguracionAplicacion>();

                return AddDocumentMetadataDbContextFactory.Crear(
                    configuracionAplicacion.ConfiguracionGeneral.ConfiguracionContpaqiContabilidad.ContpaqiSqlConnectionString,
                    configuracionAplicacion.ConfiguracionGeneral.ConfiguracionContpaqiContabilidad.Empresa.GuidAdd);
            });

            serviceCollection.AddTransient<IComprobanteAddContabilidadRepository, ComprobanteAddRepository>();
            serviceCollection.AddTransient<IComprobanteAddComercialRepository, ComprobanteAddRepository>();
        }
    }
}
