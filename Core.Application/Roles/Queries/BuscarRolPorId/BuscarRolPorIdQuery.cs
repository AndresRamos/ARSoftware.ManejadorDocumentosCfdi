using Core.Application.Roles.Models;
using MediatR;

namespace Core.Application.Roles.Queries.BuscarRolPorId;

public sealed record BuscarRolPorIdQuery(int RolId) : IRequest<RolDto>;
