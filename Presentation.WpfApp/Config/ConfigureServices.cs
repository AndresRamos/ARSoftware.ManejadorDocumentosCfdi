using Autofac;
using Caliburn.Micro;
using MahApps.Metro.Controls.Dialogs;
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
    public static class ConfigureServices
    {
        public static ContainerBuilder AddWpfAppServices(this ContainerBuilder containerBuilder)
        {
            AddCaliburnMicroModuleServices(containerBuilder);
            AddMahappsServices(containerBuilder);
            AddViewModels(containerBuilder);
            return containerBuilder;
        }

        private static void AddCaliburnMicroModuleServices(ContainerBuilder containerBuilder)
        {
            //  register the single window manager for this container
            containerBuilder.Register<IWindowManager>(c => new WindowManager()).InstancePerLifetimeScope();
            containerBuilder.Register<IEventAggregator>(c => new EventAggregator()).InstancePerLifetimeScope();
        }

        private static void AddMahappsServices(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterInstance(DialogCoordinator.Instance).ExternallyOwned();
        }

        private static void AddViewModels(ContainerBuilder containerBuilder)
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
    }
}
