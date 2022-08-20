using Core.Application.ConfiguracionGeneral.Models;
using MediatR;

namespace Core.Application.ConfiguracionGeneral.Commands.ActualizarConfiguracionContpaqiContabilidad
{
    public class ActualizarConfiguracionContpaqiContabilidadCommand : IRequest
    {
        public ActualizarConfiguracionContpaqiContabilidadCommand(ConfiguracionContpaqiContabilidadDto configuracionContpaqiContabilidad)
        {
            ConfiguracionContpaqiContabilidad = configuracionContpaqiContabilidad;
        }

        public ConfiguracionContpaqiContabilidadDto ConfiguracionContpaqiContabilidad { get; }
    }
}
