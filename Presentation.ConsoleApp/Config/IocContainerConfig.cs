using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Core.Application.Cfdis.Queries.ObtenerCertificado;
using Core.Application.Comprobantes.Interfaces;
using Core.Application.Empresas.Interfaces;
using Core.Application.Rfcs.Interfaces;
using Infrastructure.ContpaqiAdd.Factories;
using Infrastructure.ContpaqiAdd.Repositories;
using Infrastructure.ContpaqiComercial.Factories;
using Infrastructure.ContpaqiComercial.Repositories;
using Infrastructure.ContpaqiContabilidad.Factories;
using Infrastructure.ContpaqiContabilidad.Repositories;
using Infrastructure.Persistance;
using MediatR;
using Presentation.ConsoleApp.Models;

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

            var mediatrOpenTypes = new[]
            {
                typeof(IRequestHandler<,>),
                typeof(INotificationHandler<>)
            };

            foreach (var mediatrOpenType in mediatrOpenTypes)
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
                return ComercialGeneralesDbContextFactory.Crear(
                    configuracionAplicacion.ConfiguracionGeneral.ConfiguracionContpaqiComercial.ContpaqiSqlConnectionString);
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
                return ContabilidadGeneralesDbContextFactory.Crear(
                    configuracionAplicacion.ConfiguracionGeneral.ConfiguracionContpaqiContabilidad.ContpaqiSqlConnectionString);
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

                if (string.IsNullOrWhiteSpace(configuracionAplicacion.ConfiguracionGeneral.ConfiguracionContpaqiContabilidad.Empresa?.GuidAdd))
                {
                    return null;
                }

                var addDocumentMetadataDbContext = AddDocumentMetadataDbContextFactory.Crear(configuracionAplicacion.ConfiguracionGeneral.ConfiguracionContpaqiContabilidad.ContpaqiSqlConnectionString, configuracionAplicacion.ConfiguracionGeneral.ConfiguracionContpaqiContabilidad.Empresa.GuidAdd);
                return new ComprobanteAddRepository(addDocumentMetadataDbContext);
            }).As<IComprobanteAddContabilidadRepository>();

            containerBuilder.Register(context =>
            {
                var configuracionAplicacion = context.Resolve<ConfiguracionAplicacion>();
                if (string.IsNullOrWhiteSpace(configuracionAplicacion.ConfiguracionGeneral.ConfiguracionContpaqiComercial.Empresa?.GuidAdd))
                {
                    return null;
                }

                var addDocumentMetadataDbContext = AddDocumentMetadataDbContextFactory.Crear(configuracionAplicacion.ConfiguracionGeneral.ConfiguracionContpaqiContabilidad.ContpaqiSqlConnectionString, configuracionAplicacion.ConfiguracionGeneral.ConfiguracionContpaqiComercial.Empresa.GuidAdd);
                return new ComprobanteAddRepository(addDocumentMetadataDbContext);
            }).As<IComprobanteAddComercialRepository>();
        }
    }
}
