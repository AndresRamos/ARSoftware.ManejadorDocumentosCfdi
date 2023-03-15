using MediatR;

namespace Core.Application.Usuarios.Commands.CambiarContrasena;

public sealed record CambiarContrasenaCommand(int UsuarioId, string PasswordNueva) : IRequest;
