using MediatR;

namespace Core.Application.Usuarios.Commands.RemoverRol;

public sealed record RemoverRolCommand(int UsuarioId, int RolId) : IRequest;
