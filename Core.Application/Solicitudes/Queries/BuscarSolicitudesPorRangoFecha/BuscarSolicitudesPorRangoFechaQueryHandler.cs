using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Application.Solicitudes.Models;
using Infrastructure.Persistance;
using MediatR;

namespace Core.Application.Solicitudes.Queries.BuscarSolicitudesPorRangoFecha
{
    public class BuscarSolicitudesPorRangoFechaQueryHandler : IRequestHandler<BuscarSolicitudesPorRangoFechaQuery, IEnumerable<SolicitudDto>>
    {
        private readonly ManejadorDocumentosCfdiDbContext _context;

        public BuscarSolicitudesPorRangoFechaQueryHandler(ManejadorDocumentosCfdiDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SolicitudDto>> Handle(BuscarSolicitudesPorRangoFechaQuery request, CancellationToken cancellationToken)
        {
            var solicitudes = await _context.Solicitudes.Where(s => s.FechaCreacion >= request.FechaInicio && s.FechaCreacion <= request.FechaFin).ToListAsync(cancellationToken);

            return solicitudes.Select(s => new SolicitudDto
            {
                Id = s.Id,
                FechaCreacion = s.FechaCreacion,
                FechaInicio = s.FechaInicio,
                FechaFin = s.FechaFin,
                Token = s.Token
            }).ToList();
        }
    }
}