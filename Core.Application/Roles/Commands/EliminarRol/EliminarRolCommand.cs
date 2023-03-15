using MediatR;

namespace Core.Application.Roles.Commands.EliminarRol;

public sealed record EliminarRolCommand(int RolId) : IRequest;
