using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Caliburn.Micro;
using Core.Application.Permisos.Models;
using MahApps.Metro.Controls.Dialogs;
using Presentation.WpfApp.Models;
using Presentation.WpfApp.ViewModels.Actualizaciones;
using Presentation.WpfApp.ViewModels.Autenticacion;
using Presentation.WpfApp.ViewModels.ConfiguracionGeneral;
using Presentation.WpfApp.ViewModels.Roles;
using Presentation.WpfApp.ViewModels.Solicitudes;
using Presentation.WpfApp.ViewModels.Usuarios;

namespace Presentation.WpfApp.ViewModels
{
    public sealed class ShellViewModel : Conductor<Screen>.Collection.OneActive
    {
        private readonly IDialogCoordinator _dialogCoordinator;
        private readonly IWindowManager _windowManager;

        public ShellViewModel(IWindowManager windowManager, IDialogCoordinator dialogCoordinator, ConfiguracionAplicacion configuracionAplicacion)
        {
            _windowManager = windowManager;
            _dialogCoordinator = dialogCoordinator;
            ConfiguracionAplicacion = configuracionAplicacion;
            DisplayName = "AR Software - Manejador Documentos CFDI";
        }

        public ConfiguracionAplicacion ConfiguracionAplicacion { get; }

        public bool CanIniciarSesionAsync => !ConfiguracionAplicacion.IsUsuarioAutenticado;
        public bool CanCerrarSesionAsync => ConfiguracionAplicacion.IsUsuarioAutenticado;
        public bool CanVerListaSolicitudesViewAsync => ConfiguracionAplicacion.IsUsuarioAutenticado && ConfiguracionAplicacion.Usuario.TienePermiso(PermisosAplicacion.PuedeVerListaSolicitudes);
        public bool CanVerConfiguracionGeneralViewAsync => ConfiguracionAplicacion.IsUsuarioAutenticado && ConfiguracionAplicacion.Usuario.TienePermiso(PermisosAplicacion.PuedeEditarConfiguracionGeneral);
        public bool CanVerListaRolesViewAsync => ConfiguracionAplicacion.IsUsuarioAutenticado && ConfiguracionAplicacion.Usuario.TienePermiso(PermisosAplicacion.PuedeEditarUsuarios);
        public bool CanVerListaUsuariosViewAsync => ConfiguracionAplicacion.IsUsuarioAutenticado && ConfiguracionAplicacion.Usuario.TienePermiso(PermisosAplicacion.PuedeEditarUsuarios);

        public void Salir()
        {
            TryClose();
        }

        public async Task IniciarSesionAsync()
        {
            try
            {
                var viewModel = IoC.Get<AutenticarUsuarioViewModel>();
                viewModel.Inicializar();
                _windowManager.ShowDialog(viewModel);
                if (viewModel.IsUsuarioAutenticado)
                {
                    ConfiguracionAplicacion.SetUsuario(viewModel.Usuario);
                }
            }
            catch (Exception e)
            {
                await _dialogCoordinator.ShowMessageAsync(this, "Error", e.ToString());
            }
            finally
            {
                RaiseGuards();
            }
        }

        public async Task CerrarSesionAsync()
        {
            try
            {
                Items.Clear();
                ConfiguracionAplicacion.SetUsuario(null);
            }
            catch (Exception e)
            {
                await _dialogCoordinator.ShowMessageAsync(this, "Error", e.ToString());
            }
            finally
            {
                RaiseGuards();
            }
        }

        public async Task VerListaSolicitudesViewAsync()
        {
            try
            {
                var viewModel = IoC.Get<ListaSolicitudesViewModel>();
                Items.Add(viewModel);
                ActivateItem(viewModel);
            }
            catch (Exception e)
            {
                await _dialogCoordinator.ShowMessageAsync(this, "Error", e.ToString());
            }
        }

        public async Task VerConfiguracionGeneralViewAsync()
        {
            try
            {
                var viewModel = IoC.Get<ConfiguracionGeneralViewModel>();
                await viewModel.InicializarAsync();
                _windowManager.ShowWindow(viewModel);
            }
            catch (Exception e)
            {
                await _dialogCoordinator.ShowMessageAsync(this, "Error", e.ToString());
            }
        }

        public async Task VerListaRolesViewAsync()
        {
            try
            {
                var viewModel = IoC.Get<ListaRolesViewModel>();
                await viewModel.InicializarAsync();
                _windowManager.ShowDialog(viewModel);
            }
            catch (Exception e)
            {
                await _dialogCoordinator.ShowMessageAsync(this, "Error", e.ToString());
            }
        }

        public async Task VerListaUsuariosViewAsync()
        {
            try
            {
                var viewModel = IoC.Get<ListaUsuariosViewModel>();
                await viewModel.InicializarAsync();
                _windowManager.ShowDialog(viewModel);
            }
            catch (Exception e)
            {
                await _dialogCoordinator.ShowMessageAsync(this, "Error", e.ToString());
            }
        }

        public async Task BuscarActualizacionesAsync()
        {
            try
            {
                var viewModel = IoC.Get<ActualizacionAplicacionViewModel>();
                await viewModel.ChecarActualizacionDisponibleAsync();
                _windowManager.ShowDialog(viewModel);
            }
            catch (Exception ex)
            {
                await _dialogCoordinator.ShowMessageAsync(this, "Error", ex.Message);
            }
            finally
            {
                RaiseGuards();
            }
        }

        public async Task IniciarSoporteRemotoAsync()
        {
            try
            {
                Process.Start(@"https://get.teamviewer.com/ar_software_quick_support");
            }
            catch (Exception e)
            {
                await _dialogCoordinator.ShowMessageAsync(this, "Error", e.Message);
            }
        }

        public async Task VerAcercaDeViewAsync()
        {
            try
            {
                var viewModel = IoC.Get<AcercaDeViewModel>();
                _windowManager.ShowDialog(viewModel);
            }
            catch (Exception ex)
            {
                await _dialogCoordinator.ShowMessageAsync(this, "Error", ex.ToString());
            }
        }

        public async Task VerDocumentacionAsync()
        {
            try
            {
                Process.Start("https://www.arsoft.net/mdcfdi-acerca-de");
            }
            catch (Exception e)
            {
                await _dialogCoordinator.ShowMessageAsync(this, "Error", e.ToString());
            }
        }

        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            var viewModel = IoC.Get<ActualizacionAplicacionViewModel>();
            await viewModel.ChecarActualizacionDisponibleAsync();
            if (viewModel.ActualizacionAplicacion.ActualizacionDisponible)
            {
                _windowManager.ShowWindow(viewModel);
            }

            await ConfiguracionAplicacion.CargarConfiguracionAsync();
        }

        private void RaiseGuards()
        {
            NotifyOfPropertyChange(() => CanIniciarSesionAsync);
            NotifyOfPropertyChange(() => CanCerrarSesionAsync);
            NotifyOfPropertyChange(() => CanVerListaSolicitudesViewAsync);
            NotifyOfPropertyChange(() => CanVerConfiguracionGeneralViewAsync);
            NotifyOfPropertyChange(() => CanVerListaRolesViewAsync);
            NotifyOfPropertyChange(() => CanVerListaUsuariosViewAsync);
        }
    }
}