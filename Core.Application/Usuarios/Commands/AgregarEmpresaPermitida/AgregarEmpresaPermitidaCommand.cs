using MediatR;

namespace Core.Application.Usuarios.Commands.AgregarEmpresaPermitida
{
    public class AgregarEmpresaPermitidaCommand : IRequest
    {
        public AgregarEmpresaPermitidaCommand(int usuarioId, int empresaId)
        {
            UsuarioId = usuarioId;
            EmpresaId = empresaId;
        }

        public int UsuarioId { get; }
        public int EmpresaId { get; }
    }
}