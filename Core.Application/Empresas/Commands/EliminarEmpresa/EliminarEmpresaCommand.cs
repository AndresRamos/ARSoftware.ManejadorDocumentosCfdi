using MediatR;

namespace Core.Application.Empresas.Commands.EliminarEmpresa;

public class EliminarEmpresaCommand : IRequest
{
    public EliminarEmpresaCommand(int empresaId)
    {
        EmpresaId = empresaId;
    }

    public int EmpresaId { get; }
}
