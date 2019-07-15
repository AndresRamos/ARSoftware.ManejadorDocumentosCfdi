using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Persistance;
using Infrastructure.Sat;
using Infrastructure.Sat.Services;
using MediatR;

namespace Core.Application.Solicitudes.Commands.GenerarSolicitud
{
    public class GenerarSolicitudCommandHandler : IRequestHandler<GenerarSolicitudCommand, Unit>
    {
        private readonly ManejadorDocumentosCfdiDbContext _context;

        public GenerarSolicitudCommandHandler(ManejadorDocumentosCfdiDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(GenerarSolicitudCommand request, CancellationToken cancellationToken)
        {
            var configuracionGeneral = await _context.ConfiguracionGeneral.FirstAsync(cancellationToken);
            var solicitud = await _context.Solicitudes.SingleAsync(s => s.Id == request.SolicitudId, cancellationToken);

            var certificadoSat = CertificadoService.ObtenerCertificado(configuracionGeneral.CertificadoSat.Certificado, configuracionGeneral.CertificadoSat.Contrasena);

            var solicitudService = new GenerarSolicitudService(UrlsSat.UrlSolicitud, UrlsSat.UrlSolicitudAction);
            var xmlSolicitud = solicitudService.Generate(certificadoSat,
                configuracionGeneral.CertificadoSat.RfcEmisor,
                "",
                configuracionGeneral.CertificadoSat.RfcEmisor,
                solicitud.FechaInicio.ToString("yyyy-MM-dd"),
                solicitud.FechaFin.ToString("yyyy-MM-dd"));

            solicitud.SolicitudSatId = solicitudService.Send(solicitud.Autorizacion);

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}