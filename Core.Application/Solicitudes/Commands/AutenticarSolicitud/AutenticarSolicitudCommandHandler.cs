using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Core.Domain.Enums;
using Infrastructure.Persistance;
using Infrastructure.Sat;
using Infrastructure.Sat.Services;
using MediatR;
using NLog;

namespace Core.Application.Solicitudes.Commands.AutenticarSolicitud
{
    public class AutenticarSolicitudCommandHandler : IRequestHandler<AutenticarSolicitudCommand, Unit>
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ManejadorDocumentosCfdiDbContext _context;

        public AutenticarSolicitudCommandHandler(ManejadorDocumentosCfdiDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(AutenticarSolicitudCommand request, CancellationToken cancellationToken)
        {
            Logger.Info("Autenticando solicitud {0}", request.SolicitudId);

            var configuracionGeneral = await _context.ConfiguracionGeneral.FirstAsync(cancellationToken);
            var solicitud = await _context.Solicitudes.SingleAsync(s => s.Id == request.SolicitudId, cancellationToken);

            Logger.Info("Obteniendo certificado.");
            var certificadoSat = CertificadoService.ObtenerCertificado(configuracionGeneral.CertificadoSat.Certificado, configuracionGeneral.CertificadoSat.Contrasena);

            var service = new AutenticacionService(UrlsSat.UrlAutentica, UrlsSat.UrlAutenticaAction);

            Logger.Info("Generando XML.");
            var xml = service.Generate(certificadoSat);
            Logger.Info(xml);

            Logger.Info("Enviando");
            var solicitudResult = service.Send(xml, null);

            solicitud.Token = solicitudResult.Token;
            solicitud.Autorizacion = $"WRAP access_token=\"{HttpUtility.UrlDecode(solicitudResult.Token)}\"";

            Logger.Info("Token: {0}", solicitud.Token);
            Logger.Info("Autorizacion: {0}", solicitud.Autorizacion);

            solicitud.Estatus = EstatusSolicitud.Autenticada;

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}