using System.Reflection;
using Autofac;
using Caliburn.Micro;
using Core.Application.Cfdis.Queries.ObtenerCertificado;
using Core.Application.Empresas.Interfaces;
using Core.Application.Rfcs.Interfaces;
using Infrastructure.ContpaqiComercial.Factories;
using Infrastructure.ContpaqiComercial.Repositories;
using Infrastructure.ContpaqiContabilidad.Factories;
using Infrastructure.ContpaqiContabilidad.Repositories;
using Infrastructure.Persistance;
using MahApps.Metro.Controls.Dialogs;
using MediatR;
using Presentation.WpfApp.Models;
using Presentation.WpfApp.ViewModels;
using Presentation.WpfApp.ViewModels.Actualizaciones;
using Presentation.WpfApp.ViewModels.ConfiguracionGeneral;
using Presentation.WpfApp.ViewModels.Empresas;
using Presentation.WpfApp.ViewModels.Rfcs;
using Presentation.WpfApp.ViewModels.Solicitudes;
using Presentation.WpfApp.ViewModels.Xmls;

namespace Presentation.WpfApp.Config
{
    public static class IocContainerConfig
    {
        public static IContainer Configure()
        {
            var containerBuilder = new ContainerBuilder();

            RegisterCaliburnMicroModule(containerBuilder);
            RegisterMahappsModule(containerBuilder);
            RegisterMediatr(containerBuilder);
            RegisterModels(containerBuilder);
            RegisterViewModels(containerBuilder);
            RegisterPersistanceModule(containerBuilder);
            RegisterContpaqiComercialModule(containerBuilder);
            RegisterContpaqiContabilidadModule(containerBuilder);

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

            containerBuilder.RegisterType<ActualizacionAplicacionViewModel>();

            containerBuilder.RegisterType<ConfiguracionGeneralViewModel>();

            containerBuilder.RegisterType<SeleccionarEmpresaContpaqiViewModel>();

            containerBuilder.RegisterType<SeleccionarRfcViewModel>();

            containerBuilder.RegisterType<DetalleSolicitudViewModel>();
            containerBuilder.RegisterType<ListaSolicitudesViewModel>();
            containerBuilder.RegisterType<NuevaSolicitudViewModel>();
            containerBuilder.RegisterType<SolicitudAutenticacionViewModel>();
            containerBuilder.RegisterType<SolicitudDescargaViewModel>();
            containerBuilder.RegisterType<SolicitudPaquetesViewModel>();
            containerBuilder.RegisterType<SolicitudSolicitudViewModel>();
            containerBuilder.RegisterType<SolicitudVerificacionViewModel>();
            containerBuilder.RegisterType<SolicitudViewModel>();

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
    }
}