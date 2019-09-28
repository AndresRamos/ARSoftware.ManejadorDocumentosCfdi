using System.Threading.Tasks;
using Caliburn.Micro;
using MediatR;
using Presentation.WpfApp.ViewModels.ConfiguracionGeneral;
using Presentation.WpfApp.ViewModels.Solicitudes;

namespace Presentation.WpfApp.ViewModels
{
    public sealed class ShellViewModel : Conductor<Screen>.Collection.OneActive
    {
        private readonly IMediator _mediator;
        private readonly IWindowManager _windowManager;

        public ShellViewModel(IMediator mediator, IWindowManager windowManager, ListaSolicitudesViewModel listaSolicitudesViewModel)
        {
            _mediator = mediator;
            _windowManager = windowManager;
            DisplayName = "AR Software - Manejador Documentos CFDI";
            Items.Add(listaSolicitudesViewModel);
        }

        public async Task MostrarConfiguracionGeneralAsync()
        {
            var viewModel = IoC.Get<ConfiguracionGeneralViewModel>();
            await viewModel.InicializarAsync();
            _windowManager.ShowWindow(viewModel);
        }
    }
}