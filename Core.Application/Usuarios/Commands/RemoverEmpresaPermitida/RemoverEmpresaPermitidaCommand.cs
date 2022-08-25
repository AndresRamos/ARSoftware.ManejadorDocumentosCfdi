using MediatR;

namespace Core.Application.Usuarios.Commands.RemoverEmpresaPermitida;

public class RemoverEmpresaPermitidaCommand : IRequest
{
    public RemoverEmpresaPermitidaCommand(int usuarioId, int empresaId)
    {
        UsuarioId = usuarioId;
        EmpresaId = empresaId;
    }

    public int UsuarioId { get; }
    public int EmpresaId { get; }
}
