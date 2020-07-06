using MediatR;

namespace Core.Application.Usuarios.Commands.AgregarRol
{
    public class AgregarRolCommand : IRequest
    {
        public AgregarRolCommand(int usuarioId, int rolId)
        {
            UsuarioId = usuarioId;
            RolId = rolId;
        }

        public int UsuarioId { get; }
        public int RolId { get; }
    }
}