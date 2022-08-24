using ARSoftware.Contpaqi.Add.Sql.Contexts;
using ARSoftware.Contpaqi.Add.Sql.Factories;
using ARSoftware.Contpaqi.Comercial.Sql.Contexts;
using ARSoftware.Contpaqi.Comercial.Sql.Factories;
using ARSoftware.Contpaqi.Contabilidad.Sql.Contexts;
using ARSoftware.Contpaqi.Contabilidad.Sql.Factories;
using Core.Application.Common;
using Core.Application.Comprobantes.Interfaces;
using Core.Application.Empresas.Interfaces;
using Core.Application.Rfcs.Interfaces;
using Infrastructure.Contpaqi.ADD.Repositories;
using Infrastructure.Contpaqi.Comercial.Repositories;
using Infrastructure.Contpaqi.Contabilidad.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Common;

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
        serviceCollection.AddDbContext<ContpaqiComercialGeneralesDbContext>((provider, builder) =>
        {
            var configuracionAplicacion = provider.GetRequiredService<ConfiguracionAplicacion>();
            string connectionString =
                ContpaqiComercialSqlConnectionStringFactory.CreateContpaqiComercialGeneralesConnectionString(configuracionAplicacion
                    .ConfiguracionGeneral.ConfiguracionContpaqiComercial.ContpaqiSqlConnectionString);
            builder.UseSqlServer(connectionString);
        });

        serviceCollection.AddDbContext<ContpaqiComercialEmpresaDbContext>((provider, builder) =>
        {
            var configuracionAplicacion = provider.GetRequiredService<ConfiguracionAplicacion>();
            string connectionString = ContpaqiComercialSqlConnectionStringFactory.CreateContpaqiComercialEmpresaConnectionString(
                configuracionAplicacion.ConfiguracionGeneral.ConfiguracionContpaqiComercial.ContpaqiSqlConnectionString,
                configuracionAplicacion.ConfiguracionGeneral.ConfiguracionContpaqiComercial.Empresa.BaseDatos);
            builder.UseSqlServer(connectionString);
        });

        serviceCollection.AddTransient<IEmpresaComercialRepository, EmpresaComercialRepository>();
        serviceCollection.AddTransient<IRfcComercialRepository, RfcComercialRepository>();
    }

    private static void AddContpaqiContabilidadServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddDbContext<ContpaqiContabilidadGeneralesDbContext>((provider, builder) =>
        {
            var configuracionAplicacion = provider.GetRequiredService<ConfiguracionAplicacion>();
            string connectionString = ContpaqiContabilidadSqlConnectionStringFactory.CreateContpaqiContabilidadGeneralesConnectionString(
                configuracionAplicacion.ConfiguracionGeneral.ConfiguracionContpaqiContabilidad.ContpaqiSqlConnectionString);
            builder.UseSqlServer(connectionString);
        });

        serviceCollection.AddDbContext<ContpaqiContabilidadEmpresaDbContext>((provider, builder) =>
        {
            var configuracionAplicacion = provider.GetRequiredService<ConfiguracionAplicacion>();
            string connectionString = ContpaqiContabilidadSqlConnectionStringFactory.CreateContpaqiContabilidadEmpresaConnectionString(
                configuracionAplicacion.ConfiguracionGeneral.ConfiguracionContpaqiContabilidad.ContpaqiSqlConnectionString,
                configuracionAplicacion.ConfiguracionGeneral.ConfiguracionContpaqiContabilidad.Empresa.BaseDatos);
            builder.UseSqlServer(connectionString);
        });

        serviceCollection.AddTransient<IEmpresaContabilidadRepository, EmpresaContabilidadRepository>();
        serviceCollection.AddTransient<IRfcContabilidadRepository, RfcContabilidadRepository>();
    }

    private static void AddContpaqiAddServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddDbContext<ContpaqiAddDocumentMetadataDbContext>((provider, builder) =>
        {
            var configuracionAplicacion = provider.GetRequiredService<ConfiguracionAplicacion>();
            string connectionString = ContpaqiAddSqlConnectionStringFactory.CreateContpaqiAddDocumentMetadataConnectionString(
                configuracionAplicacion.ConfiguracionGeneral.ConfiguracionContpaqiContabilidad.ContpaqiSqlConnectionString,
                configuracionAplicacion.ConfiguracionGeneral.ConfiguracionContpaqiContabilidad.Empresa.GuidAdd);
            builder.UseSqlServer(connectionString);
        });

        serviceCollection.AddTransient<IComprobanteAddContabilidadRepository, ComprobanteAddRepository>();
        serviceCollection.AddTransient<IComprobanteAddComercialRepository, ComprobanteAddRepository>();
    }
}
