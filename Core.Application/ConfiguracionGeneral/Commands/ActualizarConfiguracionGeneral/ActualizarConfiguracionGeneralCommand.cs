using Core.Application.ConfiguracionGeneral.Models;
using MediatR;

namespace Core.Application.ConfiguracionGeneral.Commands.ActualizarConfiguracionGeneral;

public sealed class ActualizarConfiguracionGeneralCommand : IRequest
{
    public ActualizarConfiguracionGeneralCommand(int empresaId, ConfiguracionGeneralDto configuracionGeneral)
    {
        EmpresaId = empresaId;
        ConfiguracionGeneral = configuracionGeneral;
    }

    public int EmpresaId { get; }
    public ConfiguracionGeneralDto ConfiguracionGeneral { get; }
}
