using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Infrastructure.Persistance;
using Infrastructure.Sat;
using Infrastructure.Sat.Services;
using MediatR;

namespace Core.Application.Solicitudes.Commands.AutenticarSolicitud
{
    public class AutenticarSolicitudCommandHandler : IRequestHandler<AutenticarSolicitudCommand, Unit>
    {
        private readonly ManejadorDocumentosCfdiDbContext _context;

        public AutenticarSolicitudCommandHandler(ManejadorDocumentosCfdiDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(AutenticarSolicitudCommand request, CancellationToken cancellationToken)
        {
            var configuracionGeneral = await _context.ConfiguracionGeneral.FirstAsync(cancellationToken);
            var solicitud = await _context.Solicitudes.SingleAsync(s => s.Id == request.SolicitudId, cancellationToken);

            var service = new AutenticacionService(UrlsSat.UrlAutentica, UrlsSat.UrlAutenticaAction);
            var certificadoSat = CertificadoService.ObtenerCertificado(configuracionGeneral.CertificadoSat.Certificado, configuracionGeneral.CertificadoSat.Contrasena);
            var xml = service.Generate(certificadoSat);
            var token = service.Send();

            solicitud.Token = token;
            solicitud.Autorizacion = $"WRAP access_token=\"{HttpUtility.UrlDecode(token)}\"";
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}