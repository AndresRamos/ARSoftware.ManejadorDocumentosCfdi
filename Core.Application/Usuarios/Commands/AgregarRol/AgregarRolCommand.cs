using MediatR;

namespace Core.Application.Usuarios.Commands.AgregarRol;

public sealed record AgregarRolCommand(int UsuarioId, int RolId) : IRequest;
