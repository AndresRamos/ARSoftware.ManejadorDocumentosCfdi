using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Caliburn.Micro;
using MahApps.Metro.Controls.Dialogs;
using MediatR;
using Presentation.WpfApp.ViewModels.Actualizaciones;
using Presentation.WpfApp.ViewModels.ConfiguracionGeneral;
using Presentation.WpfApp.ViewModels.Solicitudes;

namespace Presentation.WpfApp.ViewModels
{
    public sealed class ShellViewModel : Conductor<Screen>.Collection.OneActive
    {
        private readonly IDialogCoordinator _dialogCoordinator;
        private readonly IMediator _mediator;
        private readonly IWindowManager _windowManager;

        public ShellViewModel(IMediator mediator, IWindowManager windowManager, ListaSolicitudesViewModel listaSolicitudesViewModel, IDialogCoordinator dialogCoordinator)
        {
            _mediator = mediator;
            _windowManager = windowManager;
            _dialogCoordinator = dialogCoordinator;
            DisplayName = "AR Software - Manejador Documentos CFDI";
            Items.Add(listaSolicitudesViewModel);
        }

        public void Salir()
        {
            TryClose();
        }

        public async Task MostrarConfiguracionGeneralAsync()
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

        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            var viewModel = IoC.Get<ActualizacionAplicacionViewModel>();
            await viewModel.ChecarActualizacionDisponibleAsync();
            if (viewModel.ActualizacionAplicacion.ActualizacionDisponible)
            {
                _windowManager.ShowWindow(viewModel);
            }
        }

        private void RaiseGuards()
        {
        }
    }
}