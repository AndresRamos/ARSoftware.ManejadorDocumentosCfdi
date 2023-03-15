using MediatR;

namespace Core.Application.Usuarios.Commands.CrearUsuario;

public sealed record CrearUsuarioCommand
    (string PrimerNombre, string Apellido, string Email, string NombreUsuario, string Password) : IRequest<int>;
