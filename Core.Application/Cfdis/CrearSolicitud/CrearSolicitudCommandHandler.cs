using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using Core.Domain.Entities;
using Infrastructure.Persistance;
using MediatR;

namespace Core.Application.Cfdis.CrearSolicitud
{
    public class CrearSolicitudCommandHandler : IRequestHandler<CrearSolicitudCommand, int>
    {
        private readonly ManejadorDocumentosCfdiDbContext _context;

        public CrearSolicitudCommandHandler(ManejadorDocumentosCfdiDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(CrearSolicitudCommand request, CancellationToken cancellationToken)
        {
            var nuevaSolicitud = new Solicitud();
            nuevaSolicitud.FechaInicio = request.FechaInicio;
            nuevaSolicitud.FechaFin = request.FechaFin;

            _context.Entry(nuevaSolicitud).State = EntityState.Added;
            await _context.SaveChangesAsync(cancellationToken);

            return nuevaSolicitud.Id;
        }
    }
}