using System.Reflection;
using Autofac;
using Caliburn.Micro;
using Core.Application.Cfdis.Queries.ObtenerCertificado;
using Infrastructure.Persistance;
using MahApps.Metro.Controls.Dialogs;
using MediatR;
using Presentation.WpfApp.ViewModels;
using Presentation.WpfApp.ViewModels.Actualizaciones;
using Presentation.WpfApp.ViewModels.ConfiguracionGeneral;
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
            RegisterViewModels(containerBuilder);
            RegisterPersistanceModule(containerBuilder);

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

        private static void RegisterViewModels(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<ShellViewModel>();
            containerBuilder.RegisterType<AcercaDeViewModel>();

            containerBuilder.RegisterType<ActualizacionAplicacionViewModel>();

            containerBuilder.RegisterType<ConfiguracionGeneralViewModel>();

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
    }
}