using Core.Application.ConfiguracionGeneral.Models;
using MediatR;

namespace Core.Application.ConfiguracionGeneral.Commands.ActualizarConfiguracionContpaqiComercial;

public class ActualizarConfiguracionContpaqiComercialCommand : IRequest
{
    public ActualizarConfiguracionContpaqiComercialCommand(ConfiguracionContpaqiComercialDto configuracionContpaqiComercial)
    {
        ConfiguracionContpaqiComercial = configuracionContpaqiComercial;
    }

    public ConfiguracionContpaqiComercialDto ConfiguracionContpaqiComercial { get; set; }
}
