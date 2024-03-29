﻿using ARSoftware.Cfdi.DescargaMasiva.Enumerations;
using Core.Application.TiposSolicitud.Models;
using MediatR;

namespace Core.Application.TiposSolicitud.Queries.BuscarTiposSolicitud;

public sealed class BuscarTiposSolicitudQueryHandler : IRequestHandler<BuscarTiposSolicitudQuery, IEnumerable<TipoSolicitudDto>>
{
    public Task<IEnumerable<TipoSolicitudDto>> Handle(BuscarTiposSolicitudQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult<IEnumerable<TipoSolicitudDto>>(
            TipoSolicitud.List.Select(t => new TipoSolicitudDto(t.Name, t.Value)).ToList());
    }
}
