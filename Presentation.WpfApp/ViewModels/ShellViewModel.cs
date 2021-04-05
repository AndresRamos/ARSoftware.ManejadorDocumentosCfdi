using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Caliburn.Micro;
using Core.Application.Cfdis.Queries.LeerEncabezadosCfdi;
using Core.Application.Empresas.Queries.BuscarEmpresasPermitidasPorUsuario;
using Core.Application.Permisos.Models;
using MahApps.Metro.Controls.Dialogs;
using MediatR;
using Microsoft.Win32;
using Presentation.WpfApp.Models;
using Presentation.WpfApp.ViewModels.Actualizaciones;
using Presentation.WpfApp.ViewModels.Autenticacion;
using Presentation.WpfApp.ViewModels.Cfdis;
using Presentation.WpfApp.ViewModels.ConfiguracionGeneral;
using Presentation.WpfApp.ViewModels.Empresas;
using Presentation.WpfApp.ViewModels.Roles;
using Presentation.WpfApp.ViewModels.Solicitudes;
using Presentation.WpfApp.ViewModels.Usuarios;

namespace Presentation.WpfApp.ViewModels
{
    public sealed class ShellViewModel : Conductor<Screen>.Collection.OneActive
    {
        private readonly IDialogCoordinator _dialogCoordinator;

        private readonly IMediator _mediator;
        private readonly IWindowManager _windowManager;

        public ShellViewModel(IWindowManager windowManager, IDialogCoordinator dialogCoordinator, ConfiguracionAplicacion configuracionAplicacion, IMediator mediator)
        {
            _windowManager = windowManager;
            _dialogCoordinator = dialogCoordinator;
            ConfiguracionAplicacion = configuracionAplicacion;
            _mediator = mediator;
            DisplayName = "AR Software - Manejador Documentos CFDI";
        }

        public ConfiguracionAplicacion ConfiguracionAplicacion { get; }

        public bool CanIniciarSesionAsync => !ConfiguracionAplicacion.IsUsuarioAutenticado;
        public bool CanCerrarSesionAsync => ConfiguracionAplicacion.IsUsuarioAutenticado;
        public bool CanVerListaSolicitudesViewAsync => ConfiguracionAplicacion.IsUsuarioAutenticado && ConfiguracionAplicacion.Usuario.TienePermiso(PermisosAplicacion.PuedeVerListaSolicitudes) && ConfiguracionAplicacion.IsEmpresaAbierta;
        public bool CanVerConfiguracionGeneralViewAsync => ConfiguracionAplicacion.IsUsuarioAutenticado && ConfiguracionAplicacion.Usuario.TienePermiso(PermisosAplicacion.PuedeEditarConfiguracionGeneral) && ConfiguracionAplicacion.IsEmpresaAbierta;
        public bool CanVerListaRolesViewAsync => ConfiguracionAplicacion.IsUsuarioAutenticado && ConfiguracionAplicacion.Usuario.TienePermiso(PermisosAplicacion.PuedeEditarUsuarios);
        public bool CanVerListaUsuariosViewAsync => ConfiguracionAplicacion.IsUsuarioAutenticado && ConfiguracionAplicacion.Usuario.TienePermiso(PermisosAplicacion.PuedeEditarUsuarios);
        public bool CanValidarExistenciaEnAddAsync => ConfiguracionAplicacion.IsUsuarioAutenticado && ConfiguracionAplicacion.Usuario.TienePermiso(PermisosAplicacion.PuedeVerListaSolicitudes) && ConfiguracionAplicacion.IsEmpresaAbierta;
        public bool CanVerListaEmpresasViewAsync => ConfiguracionAplicacion.IsUsuarioAutenticado && ConfiguracionAplicacion.Usuario.TienePermiso(PermisosAplicacion.PuedeEditarEmpresas) && !ConfiguracionAplicacion.IsEmpresaAbierta;
        public bool CanAbrirEmpresaAsync => ConfiguracionAplicacion.IsUsuarioAutenticado && !ConfiguracionAplicacion.IsEmpresaAbierta;
        public bool CanCerrarEmpresaAsync => ConfiguracionAplicacion.IsEmpresaAbierta;

        public async Task Salir()
        {
            await TryCloseAsync();
        }

        public async Task IniciarSesionAsync()
        {
            try
            {
                var viewModel = IoC.Get<AutenticarUsuarioViewModel>();
                viewModel.Inicializar();
                await _windowManager.ShowDialogAsync(viewModel);
                if (viewModel.IsUsuarioAutenticado)
                {
                    ConfiguracionAplicacion.SetUsuario(viewModel.Usuario);

                    if (CanAbrirEmpresaAsync)
                    {
                        await AbrirEmpresaAsync();
                    }
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
                ConfiguracionAplicacion.CerrarEmpresa();
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

        public async Task AbrirEmpresaAsync()
        {
            try
            {
                var seleccionarEmpresaViewModel = IoC.Get<SeleccionarEmpresaViewModel>();

                if (ConfiguracionAplicacion.Usuario.TienePermiso(PermisosAplicacion.TodasLasEmpresasPermitidas))
                {
                    await seleccionarEmpresaViewModel.InicializarAsync();
                }
                else
                {
                    seleccionarEmpresaViewModel.Inicializar(await _mediator.Send(new BuscarEmpresasPermitidasPorUsuarioQuery(ConfiguracionAplicacion.Usuario.Id)));
                }

                await _windowManager.ShowDialogAsync(seleccionarEmpresaViewModel);
                if (seleccionarEmpresaViewModel.SeleccionoEmpresa)
                {
                    await ConfiguracionAplicacion.AbrirEmpresaAsync(seleccionarEmpresaViewModel.EmpresaSeleccionada);
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

        public async Task CerrarEmpresaAsync()
        {
            try
            {
                ConfiguracionAplicacion.CerrarEmpresa();
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
                await ActivateItemAsync(viewModel);
            }
            catch (Exception e)
            {
                await _dialogCoordinator.ShowMessageAsync(this, "Error", e.ToString());
            }
        }

        public async Task ValidarExistenciaEnAddAsync()
        {
            try
            {
                var openFileDialog = new OpenFileDialog
                {
                    Multiselect = true,
                    Filter = "XML|*.xml"
                };
                if (openFileDialog.ShowDialog() != true)
                {
                    return;
                }

                var progressDialogController = await _dialogCoordinator.ShowProgressAsync(this, "Cargando", "Cargando...");
                progressDialogController.SetIndeterminate();
                await Task.Delay(1000);

                var comprobantes = await _mediator.Send(new LeerEncabezadosCfdiQuery(openFileDialog.FileNames));
                var listaCfdisViewModel = IoC.Get<ListaCfdisViewModel>();
                listaCfdisViewModel.Inicializar(comprobantes);

                await progressDialogController.CloseAsync();

                await _windowManager.ShowDialogAsync(listaCfdisViewModel);
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
                await _windowManager.ShowWindowAsync(viewModel);
                await ConfiguracionAplicacion.CargarConfiguracionAsync();
            }
            catch (Exception e)
            {
                await _dialogCoordinator.ShowMessageAsync(this, "Error", e.ToString());
            }
        }

        public async Task VerListaEmpresasViewAsync()
        {
            try
            {
                var viewModel = IoC.Get<ListaEmpresasViewModel>();
                await viewModel.InicializarAsync();
                await ActivateItemAsync(viewModel);
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
                await _windowManager.ShowDialogAsync(viewModel);
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
                await _windowManager.ShowDialogAsync(viewModel);
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
                await _windowManager.ShowDialogAsync(viewModel);
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
                await _windowManager.ShowDialogAsync(viewModel);
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
                await _windowManager.ShowWindowAsync(viewModel);
            }
        }

        private void RaiseGuards()
        {
            NotifyOfPropertyChange(() => CanIniciarSesionAsync);
            NotifyOfPropertyChange(() => CanCerrarSesionAsync);
            NotifyOfPropertyChange(() => CanVerListaSolicitudesViewAsync);
            NotifyOfPropertyChange(() => CanVerConfiguracionGeneralViewAsync);
            NotifyOfPropertyChange(() => CanVerListaRolesViewAsync);
            NotifyOfPropertyChange(() => CanVerListaUsuariosViewAsync);
            NotifyOfPropertyChange(() => CanValidarExistenciaEnAddAsync);
            NotifyOfPropertyChange(() => CanVerListaEmpresasViewAsync);
            NotifyOfPropertyChange(() => CanAbrirEmpresaAsync);
            NotifyOfPropertyChange(() => CanCerrarEmpresaAsync);
        }
    }
}