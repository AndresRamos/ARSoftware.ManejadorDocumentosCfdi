using System.Threading.Tasks;
using Caliburn.Micro;
using Core.Application.ConfiguracionGeneral.Models;
using Core.Application.ConfiguracionGeneral.Queries.BuscarConfiguracionGeneral;
using MediatR;

namespace Presentation.WpfApp.Models
{
    public class ConfiguracionAplicacion : PropertyChangedBase
    {
        private readonly IMediator _mediator;
        private ConfiguracionGeneralDto _configuracionGeneral;

        public ConfiguracionAplicacion(IMediator mediator)
        {
            _mediator = mediator;
        }

        public ConfiguracionGeneralDto ConfiguracionGeneral
        {
            get => _configuracionGeneral;
            private set
            {
                if (Equals(value, _configuracionGeneral))
                {
                    return;
                }

                _configuracionGeneral = value;
                NotifyOfPropertyChange(() => ConfiguracionGeneral);
            }
        }

        public async Task CargarConfiguracionAsync()
        {
            ConfiguracionGeneral = await _mediator.Send(new BuscarConfiguracionGeneralQuery());
        }
    }
}