using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Persistance;
using Infrastructure.Sat;
using Infrastructure.Sat.Services;
using MediatR;

namespace Core.Application.Solicitudes.Commands.VerificarSolicitud
{
    public class VerificarSolicitudCommandHandler : IRequestHandler<VerificarSolicitudCommand, Unit>
    {
        private readonly ManejadorDocumentosCfdiDbContext _context;

        public VerificarSolicitudCommandHandler(ManejadorDocumentosCfdiDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(VerificarSolicitudCommand request, CancellationToken cancellationToken)
        {
            var configuracionGeneral = await _context.ConfiguracionGeneral.FirstAsync(cancellationToken);
            var solicitud = await _context.Solicitudes.SingleAsync(s => s.Id == request.SolicitudId, cancellationToken);

            var certificadoSat = CertificadoService.ObtenerCertificado(configuracionGeneral.CertificadoSat.Certificado, configuracionGeneral.CertificadoSat.Contrasena);

            var verifica = new VerificaSolicitudService(UrlsSat.UrlVerificarSolicitud, UrlsSat.UrlVerificarSolicitudAction);
            var xmlVerifica = verifica.Generate(certificadoSat, configuracionGeneral.CertificadoSat.RfcEmisor, solicitud.SolicitudSatId);
            solicitud.PaqueteId = verifica.Send(solicitud.Autorizacion);
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}