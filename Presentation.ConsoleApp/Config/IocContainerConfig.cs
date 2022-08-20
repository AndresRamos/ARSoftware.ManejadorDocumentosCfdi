using System;
using System.Reflection;
using Autofac;
using Contpaqi.Sql.ADD.DocumentMetadata;
using Core.Application.Cfdis.Queries.ObtenerCertificado;
using Core.Application.Common;
using Core.Application.Comprobantes.Interfaces;
using Core.Application.Empresas.Interfaces;
using Core.Application.Rfcs.Interfaces;
using Infrastructure.Contpaqi.ADD.Factories;
using Infrastructure.Contpaqi.ADD.Repositories;
using Infrastructure.Contpaqi.Comercial.Factories;
using Infrastructure.Contpaqi.Comercial.Repositories;
using Infrastructure.Contpaqi.Contabilidad.Factories;
using Infrastructure.Contpaqi.Contabilidad.Repositories;
using Infrastructure.Persistance;
using MediatR;

namespace Presentation.ConsoleApp.Config
{
    public static class IocContainerConfig
    {
        public static IContainer Configure()
        {
            var containerBuilder = new ContainerBuilder();

            RegisterMediatr(containerBuilder);
            RegisterModels(containerBuilder);
            RegisterPersistanceModule(containerBuilder);
            RegisterContpaqiComercialModule(containerBuilder);
            RegisterContpaqiContabilidadModule(containerBuilder);
            RegisterContpaqiAddModule(containerBuilder);

            return containerBuilder.Build();
        }

        private static void RegisterMediatr(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(IMediator).GetTypeInfo().Assembly).AsImplementedInterfaces();

            Type[] mediatrOpenTypes = { typeof(IRequestHandler<,>), typeof(INotificationHandler<>) };

            foreach (Type mediatrOpenType in mediatrOpenTypes)
            {
                builder.RegisterAssemblyTypes(typeof(ObtenerCertificadoQueryHandler).GetTypeInfo().Assembly)
                    .AsClosedTypesOf(mediatrOpenType)
                    .AsImplementedInterfaces();
            }

            builder.Register<ServiceFactory>(ctx =>
            {
                var c = ctx.Resolve<IComponentContext>();
                return t => c.Resolve(t);
            });
        }

        private static void RegisterModels(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<ConfiguracionAplicacion>().SingleInstance();
        }

        private static void RegisterPersistanceModule(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<ManejadorDocumentosCfdiDbContext>();
        }

        private static void RegisterContpaqiComercialModule(ContainerBuilder containerBuilder)
        {
            containerBuilder.Register(c =>
            {
                var configuracionAplicacion = c.Resolve<ConfiguracionAplicacion>();
                return ComercialGeneralesDbContextFactory.Crear(configuracionAplicacion.ConfiguracionGeneral.ConfiguracionContpaqiComercial
                    .ContpaqiSqlConnectionString);
            });

            containerBuilder.Register(c =>
            {
                var configuracionAplicacion = c.Resolve<ConfiguracionAplicacion>();
                return ComercialEmpresaDbContextFactory.Crear(
                    configuracionAplicacion.ConfiguracionGeneral.ConfiguracionContpaqiComercial.ContpaqiSqlConnectionString,
                    configuracionAplicacion.ConfiguracionGeneral.ConfiguracionContpaqiComercial.Empresa.BaseDatos);
            });

            containerBuilder.RegisterType<EmpresaComercialRepository>().As<IEmpresaComercialRepository>();
            containerBuilder.RegisterType<RfcComercialRepository>().As<IRfcComercialRepository>();
        }

        private static void RegisterContpaqiContabilidadModule(ContainerBuilder containerBuilder)
        {
            containerBuilder.Register(c =>
            {
                var configuracionAplicacion = c.Resolve<ConfiguracionAplicacion>();
                return ContabilidadGeneralesDbContextFactory.Crear(configuracionAplicacion.ConfiguracionGeneral
                    .ConfiguracionContpaqiContabilidad.ContpaqiSqlConnectionString);
            });

            containerBuilder.Register(c =>
            {
                var configuracionAplicacion = c.Resolve<ConfiguracionAplicacion>();
                return ContabilidadEmpresaDbContextFactory.Crear(
                    configuracionAplicacion.ConfiguracionGeneral.ConfiguracionContpaqiContabilidad.ContpaqiSqlConnectionString,
                    configuracionAplicacion.ConfiguracionGeneral.ConfiguracionContpaqiContabilidad.Empresa.BaseDatos);
            });

            containerBuilder.RegisterType<EmpresaContabilidadRepository>().As<IEmpresaContabilidadRepository>();
            containerBuilder.RegisterType<RfcContabilidadRepository>().As<IRfcContabilidadRepository>();
        }

        private static void RegisterContpaqiAddModule(ContainerBuilder containerBuilder)
        {
            containerBuilder.Register(c =>
                {
                    var configuracionAplicacion = c.Resolve<ConfiguracionAplicacion>();

                    if (string.IsNullOrWhiteSpace(configuracionAplicacion.ConfiguracionGeneral.ConfiguracionContpaqiContabilidad.Empresa
                            ?.GuidAdd))
                    {
                        return null;
                    }

                    AddDocumentMetadataDbContext addDocumentMetadataDbContext = AddDocumentMetadataDbContextFactory.Crear(
                        configuracionAplicacion.ConfiguracionGeneral.ConfiguracionContpaqiContabilidad.ContpaqiSqlConnectionString,
                        configuracionAplicacion.ConfiguracionGeneral.ConfiguracionContpaqiContabilidad.Empresa.GuidAdd);
                    return new ComprobanteAddRepository(addDocumentMetadataDbContext);
                })
                .As<IComprobanteAddContabilidadRepository>();

            containerBuilder.Register(context =>
                {
                    var configuracionAplicacion = context.Resolve<ConfiguracionAplicacion>();
                    if (string.IsNullOrWhiteSpace(configuracionAplicacion.ConfiguracionGeneral.ConfiguracionContpaqiComercial.Empresa
                            ?.GuidAdd))
                    {
                        return null;
                    }

                    AddDocumentMetadataDbContext addDocumentMetadataDbContext = AddDocumentMetadataDbContextFactory.Crear(
                        configuracionAplicacion.ConfiguracionGeneral.ConfiguracionContpaqiContabilidad.ContpaqiSqlConnectionString,
                        configuracionAplicacion.ConfiguracionGeneral.ConfiguracionContpaqiComercial.Empresa.GuidAdd);
                    return new ComprobanteAddRepository(addDocumentMetadataDbContext);
                })
                .As<IComprobanteAddComercialRepository>();
        }
    }
}
