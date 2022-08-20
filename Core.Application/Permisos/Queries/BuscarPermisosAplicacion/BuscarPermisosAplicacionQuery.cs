using System.Collections.Generic;
using Common.Models;
using MediatR;

namespace Core.Application.Permisos.Queries.BuscarPermisosAplicacion
{
    public class BuscarPermisosAplicacionQuery : IRequest<IEnumerable<PermisoAplicacionDto>>
    {
    }
}
