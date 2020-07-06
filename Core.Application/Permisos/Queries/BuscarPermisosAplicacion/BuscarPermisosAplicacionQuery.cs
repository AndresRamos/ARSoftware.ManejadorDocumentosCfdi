using System.Collections.Generic;
using Core.Application.Permisos.Models;
using MediatR;

namespace Core.Application.Permisos.Queries.BuscarPermisosAplicacion
{
    public class BuscarPermisosAplicacionQuery : IRequest<IEnumerable<PermisoAplicacionDto>>
    {
    }
}