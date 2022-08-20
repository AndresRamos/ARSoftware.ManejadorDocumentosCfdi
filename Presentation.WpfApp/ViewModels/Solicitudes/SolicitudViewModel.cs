using Caliburn.Micro;
using Core.Application.Solicitudes.Models;

namespace Presentation.WpfApp.ViewModels.Solicitudes
{
    public sealed class SolicitudViewModel : Screen
    {
        private SolicitudDto _solicitud;

        public SolicitudViewModel()
        {
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
