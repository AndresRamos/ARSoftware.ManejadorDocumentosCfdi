﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ARSoftware.Cfdi.DescargaMasiva.Enumerations;
using Core.Application.TiposSolicitud.Models;
using MediatR;

namespace Core.Application.TiposSolicitud.Queries.BuscarTiposSolicitud
{
    public class BuscarTiposSolicitudQueryHandler : IRequestHandler<BuscarTiposSolicitudQuery, IEnumerable<TipoSolicitudDto>>
    {
        public async Task<IEnumerable<TipoSolicitudDto>> Handle(BuscarTiposSolicitudQuery request, CancellationToken cancellationToken)
        {
            return Enumeration.GetAll<TipoSolicitud>().Select(t => new TipoSolicitudDto(t.Name, t.Id)).ToList();
        }
    }
}