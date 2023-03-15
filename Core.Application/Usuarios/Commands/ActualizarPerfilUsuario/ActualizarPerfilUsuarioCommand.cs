using MediatR;

namespace Core.Application.Usuarios.Commands.ActualizarPerfilUsuario;

public sealed record ActualizarPerfilUsuarioCommand
    (int UsuarioId, string PrimerNombre, string Apellido, string Email, string NombreUsuario) : IRequest;
