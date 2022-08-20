using MediatR;

namespace Core.Application.Usuarios.Commands.RemoverRol
{
    public class RemoverRolCommand : IRequest
    {
        public RemoverRolCommand(int usuarioId, int rolId)
        {
            UsuarioId = usuarioId;
            RolId = rolId;
        }

        public int UsuarioId { get; }
        public int RolId { get; }
    }
}
