using System;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Caliburn.Micro;
using Contpaqi.Sql.ADD.DocumentMetadata;
using Core.Application.Cfdis.Queries.ObtenerCertificado;
using Core.Application.Common;
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
using MahApps.Metro.Controls.Dialogs;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Presentation.WpfApp.Models;
using Presentation.WpfApp.ViewModels;
using Presentation.WpfApp.ViewModels.Actualizaciones;
using Presentation.WpfApp.ViewModels.Autenticacion;
using Presentation.WpfApp.ViewModels.Cfdis;
using Presentation.WpfApp.ViewModels.ConfiguracionGeneral;
using Presentation.WpfApp.ViewModels.Empresas;
using Presentation.WpfApp.ViewModels.Permisos;
using Presentation.WpfApp.ViewModels.Rfcs;
using Presentation.WpfApp.ViewModels.Roles;
using Presentation.WpfApp.ViewModels.Solicitudes;
using Presentation.WpfApp.ViewModels.Usuarios;
using Presentation.WpfApp.ViewModels.Xmls;

namespace Presentation.WpfApp.Config
{
    public static class IocContainerConfig
    {
        public static IContainer Configure()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddApplicationServices();

            var containerBuilder = new ContainerBuilder();
            containerBuilder.Populate(serviceCollection);

            RegisterCaliburnMicroModule(containerBuilder);
            RegisterMahappsModule(containerBuilder);
            RegisterMediatr(containerBuilder);
            RegisterModels(containerBuilder);
            RegisterViewModels(containerBuilder);
            RegisterPersistanceModule(containerBuilder);
            RegisterContpaqiComercialModule(containerBuilder);
            RegisterContpaqiContabilidadModule(containerBuilder);
            RegisterContpaqiAddModule(containerBuilder);

            return containerBuilder.Build();
        }

        private static void RegisterMediatr(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(IMediator).GetTypeInfo().Assembly).AsImplementedInterfaces();

            Type[] mediatrOpenTypes = new[] { typeof(IRequestHandler<,>), typeof(INotificationHandler<>) };

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

        private static void RegisterCaliburnMicroModule(ContainerBuilder containerBuilder)
        {
            //  register the single window manager for this container
            containerBuilder.Register<IWindowManager>(c => new WindowManager()).InstancePerLifetimeScope();
            //  register the single event aggregator for this container
            containerBuilder.Register<IEventAggregator>(c => new EventAggregator()).InstancePerLifetimeScope();
        }

        private static void RegisterMahappsModule(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterInstance(DialogCoordinator.Instance).ExternallyOwned();
        }

        private static void RegisterModels(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<ConfiguracionAplicacion>().SingleInstance();
        }

        private static void RegisterViewModels(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<ShellViewModel>();
            containerBuilder.RegisterType<AcercaDeViewModel>();

            // Actualizaciones
            containerBuilder.RegisterType<ActualizacionAplicacionViewModel>();

            // Autenticacion
            containerBuilder.RegisterType<AutenticarUsuarioViewModel>();

            // CFDIs
            containerBuilder.RegisterType<ListaCfdisViewModel>();

            // ConfiguracionGeneral
            containerBuilder.RegisterType<ConfiguracionGeneralViewModel>();

            // Empresas
            containerBuilder.RegisterType<EditarEmpresaViewModel>();
            containerBuilder.RegisterType<ListaEmpresasViewModel>();
            containerBuilder.RegisterType<SeleccionarEmpresaViewModel>();
            containerBuilder.RegisterType<SeleccionarEmpresaContpaqiViewModel>();

            // Permisos
            containerBuilder.RegisterType<SeleccionarPermisoAplicacionViewModel>();

            // Rfcs
            containerBuilder.RegisterType<SeleccionarRfcViewModel>();

            // Roles
            containerBuilder.RegisterType<EditarRolViewModel>();
            containerBuilder.RegisterType<ListaRolesViewModel>();
            containerBuilder.RegisterType<SeleccionarRolViewModel>();

            // Solicitudes
            containerBuilder.RegisterType<DetalleSolicitudViewModel>();
            containerBuilder.RegisterType<ListaSolicitudesViewModel>();
            containerBuilder.RegisterType<NuevaSolicitudViewModel>();
            containerBuilder.RegisterType<SolicitudAutenticacionViewModel>();
            containerBuilder.RegisterType<SolicitudDescargaViewModel>();
            containerBuilder.RegisterType<SolicitudPaquetesViewModel>();
            containerBuilder.RegisterType<SolicitudSolicitudViewModel>();
            containerBuilder.RegisterType<SolicitudVerificacionViewModel>();
            containerBuilder.RegisterType<SolicitudViewModel>();

            // Usuarios
            containerBuilder.RegisterType<CrearUsuarioViewModel>();
            containerBuilder.RegisterType<EditarUsuarioViewModel>();
            containerBuilder.RegisterType<ListaUsuariosViewModel>();

            // Xmls
            containerBuilder.RegisterType<XmlViewerViewModel>();
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
