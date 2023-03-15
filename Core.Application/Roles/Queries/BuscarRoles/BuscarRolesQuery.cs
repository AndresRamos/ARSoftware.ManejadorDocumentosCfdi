using Core.Application.Roles.Models;
using MediatR;

namespace Core.Application.Roles.Queries.BuscarRoles;

public sealed record BuscarRolesQuery : IRequest<IEnumerable<RolDto>>;
