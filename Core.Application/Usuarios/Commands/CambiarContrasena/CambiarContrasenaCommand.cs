using MediatR;

namespace Core.Application.Usuarios.Commands.CambiarContrasena;

public class CambiarContrasenaCommand : IRequest
{
    public CambiarContrasenaCommand(int usuarioId, string passwordNueva)
    {
        UsuarioId = usuarioId;
        PasswordNueva = passwordNueva;
    }

    public int UsuarioId { get; }
    public string PasswordNueva { get; }
}
