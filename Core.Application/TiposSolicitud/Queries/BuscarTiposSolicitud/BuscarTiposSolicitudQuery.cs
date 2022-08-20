using System.Collections.Generic;
using Core.Application.TiposSolicitud.Models;
using MediatR;

namespace Core.Application.TiposSolicitud.Queries.BuscarTiposSolicitud
{
    public class BuscarTiposSolicitudQuery : IRequest<IEnumerable<TipoSolicitudDto>>
    {
    }
}
