using Caliburn.Micro;
using Core.Application.Solicitudes.Models;
using MahApps.Metro.Controls.Dialogs;

namespace Presentation.WpfApp.ViewModels.Solicitudes
{
    public sealed class SolicitudViewModel : Screen
    {
        private readonly IDialogCoordinator _dialogCoordinator;
        private readonly IWindowManager _windowManager;
        private SolicitudDto _solicitud;

        public SolicitudViewModel(IDialogCoordinator dialogCoordinator, IWindowManager windowManager)
        {
            _dialogCoordinator = dialogCoordinator;
            _windowManager = windowManager;
            DisplayName = "Solicitud";
        }

        public SolicitudDto Solicitud
        {
            get => _solicitud;
            private set
            {
                if (Equals(value, _solicitud))
                {
                    return;
                }

                _solicitud = value;
                NotifyOfPropertyChange(() => Solicitud);
            }
        }

        public void Inicializar(SolicitudDto solicitud)
        {
            Solicitud = solicitud;
        }
    }
}