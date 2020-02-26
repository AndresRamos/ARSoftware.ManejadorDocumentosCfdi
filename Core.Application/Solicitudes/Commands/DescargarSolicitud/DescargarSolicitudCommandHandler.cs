using System;
using System.Data.Entity;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Core.Domain.Enums;
using Infrastructure.Persistance;
using Infrastructure.Sat;
using Infrastructure.Sat.Services;
using MediatR;
using NLog;

namespace Core.Application.Solicitudes.Commands.DescargarSolicitud
{
    public class DescargarSolicitudCommandHandler : IRequestHandler<DescargarSolicitudCommand, Unit>
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ManejadorDocumentosCfdiDbContext _context;

        public DescargarSolicitudCommandHandler(ManejadorDocumentosCfdiDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DescargarSolicitudCommand request, CancellationToken cancellationToken)
        {
            Logger.Info("Verificando solicitud {0}", request.SolicitudId);

            var configuracionGeneral = await _context.ConfiguracionGeneral.FirstAsync(cancellationToken);
            var solicitud = await _context.Solicitudes.SingleAsync(s => s.Id == request.SolicitudId, cancellationToken);

            if (solicitud.Estatus != EstatusSolicitud.Verificada)
            {
                throw new InvalidOperationException("La solicitud no esta verificada.");
            }

            Logger.Info("Obteniendo certificado.");
            var certificadoSat = CertificadoService.ObtenerCertificado(configuracionGeneral.CertificadoSat.Certificado, configuracionGeneral.CertificadoSat.Contrasena);

            var descargarSolicitud = new DescargarSolicitudService(UrlsSat.UrlDescargarSolicitud, UrlsSat.UrlDescargarSolicitudAction);
            Logger.Info("Generando XML.");
            var xml = descargarSolicitud.Generate(certificadoSat, configuracionGeneral.CertificadoSat.RfcEmisor, solicitud.PaqueteId);
            Logger.Info(xml);

            Logger.Info("Enviando");

            var solicitudResult = descargarSolicitud.Send(xml, solicitud.Autorizacion);

            Logger.Info("CodEstatus {0}", solicitudResult.CodEstatus);

            var file = Convert.FromBase64String(solicitudResult.Paquete);

            Directory.CreateDirectory(configuracionGeneral.RutaDirectorioDescargas);

            using (var fs = File.Create(configuracionGeneral.RutaDirectorioDescargas + solicitud.PaqueteId + ".zip", file.Length))
            {
                fs.Write(file, 0, file.Length);
            }

            solicitud.Estatus = EstatusSolicitud.Descargada;
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}