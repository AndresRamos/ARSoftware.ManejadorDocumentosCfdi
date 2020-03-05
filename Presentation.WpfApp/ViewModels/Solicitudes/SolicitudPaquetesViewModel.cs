using System.Collections.Generic;
using System.Threading.Tasks;
using Caliburn.Micro;
using Core.Application.Paquetes.Models;

namespace Presentation.WpfApp.ViewModels.Solicitudes
{
    public sealed class SolicitudPaquetesViewModel : Screen
    {
        private PaqueteDto _paqueteSeleccionado;

        public SolicitudPaquetesViewModel()
        {
            DisplayName = "Paquetes";
        }

        public BindableCollection<PaqueteDto> Paquetes { get; } = new BindableCollection<PaqueteDto>();

        public PaqueteDto PaqueteSeleccionado
        {
            get => _paqueteSeleccionado;
            set
            {
                if (Equals(value, _paqueteSeleccionado))
                {
                    return;
                }

                _paqueteSeleccionado = value;
                NotifyOfPropertyChange(() => PaqueteSeleccionado);
            }
        }

        public void Inicializar(IEnumerable<PaqueteDto> paquetes)
        {
            Paquetes.Clear();
            Paquetes.AddRange(paquetes);
        }

        public async Task GuardarPaqueteAsync()
        {

        }
    }
}