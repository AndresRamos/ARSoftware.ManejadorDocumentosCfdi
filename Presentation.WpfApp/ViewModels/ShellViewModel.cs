using System;
using System.Threading.Tasks;
using Caliburn.Micro;
using MahApps.Metro.Controls.Dialogs;
using MediatR;
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
    }
}