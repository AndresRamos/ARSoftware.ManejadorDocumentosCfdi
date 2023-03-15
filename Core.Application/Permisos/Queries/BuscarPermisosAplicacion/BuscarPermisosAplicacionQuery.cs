using Common.Models;
using MediatR;

namespace Core.Application.Permisos.Queries.BuscarPermisosAplicacion;

public sealed record BuscarPermisosAplicacionQuery : IRequest<IEnumerable<PermisoAplicacionDto>>;
