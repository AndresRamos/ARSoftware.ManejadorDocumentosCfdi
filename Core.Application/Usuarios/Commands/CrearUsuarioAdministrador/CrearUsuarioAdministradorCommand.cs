using MediatR;

namespace Core.Application.Usuarios.Commands.CrearUsuarioAdministrador;

public sealed record CrearUsuarioAdministradorCommand : IRequest<Unit>;
