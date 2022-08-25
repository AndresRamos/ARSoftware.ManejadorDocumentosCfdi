using MediatR;

namespace Core.Application.Usuarios.Commands.CrearUsuario;

public class CrearUsuarioCommand : IRequest<int>
{
    public CrearUsuarioCommand(string primerNombre, string apellido, string email, string nombreUsuario, string password)
    {
        PrimerNombre = primerNombre;
        Apellido = apellido;
        Email = email;
        NombreUsuario = nombreUsuario;
        Password = password;
    }

    public string PrimerNombre { get; }
    public string Apellido { get; }
    public string Email { get; }
    public string NombreUsuario { get; }
    public string Password { get; }
}
