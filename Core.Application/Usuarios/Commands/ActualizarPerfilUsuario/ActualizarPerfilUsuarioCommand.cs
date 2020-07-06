using MediatR;

namespace Core.Application.Usuarios.Commands.ActualizarPerfilUsuario
{
    public class ActualizarPerfilUsuarioCommand : IRequest
    {
        public ActualizarPerfilUsuarioCommand(int usuarioId, string primerNombre, string apellido, string email, string nombreUsuario)
        {
            UsuarioId = usuarioId;
            PrimerNombre = primerNombre;
            Apellido = apellido;
            Email = email;
            NombreUsuario = nombreUsuario;
        }

        public int UsuarioId { get; }
        public string PrimerNombre { get; }
        public string Apellido { get; }
        public string Email { get; }
        public string NombreUsuario { get; }
    }
}