using System;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using Core.Domain.Enums;
using Infrastructure.Persistance;
using Infrastructure.Sat;
using Infrastructure.Sat.Services;
using MediatR;
using NLog;

namespace Core.Application.Solicitudes.Commands.GenerarSolicitud
{
    public class GenerarSolicitudCommandHandler : IRequestHandler<GenerarSolicitudCommand, Unit>
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ManejadorDocumentosCfdiDbContext _context;

        public GenerarSolicitudCommandHandler(ManejadorDocumentosCfdiDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(GenerarSolicitudCommand request, CancellationToken cancellationToken)
        {
            Logger.Info("Generando solicitud {0}", request.SolicitudId);

            var configuracionGeneral = await _context.ConfiguracionGeneral.FirstAsync(cancellationToken);
            var solicitud = await _context.Solicitudes.SingleAsync(s => s.Id == request.SolicitudId, cancellationToken);

            if (solicitud.Estatus != EstatusSolicitud.Autenticada)
            {
                throw new InvalidOperationException("La solicitud no esta autenticada.");
            }

            Logger.Info("Obteniendo certificado.");
            var certificadoSat = CertificadoService.ObtenerCertificado(configuracionGeneral.CertificadoSat.Certificado, configuracionGeneral.CertificadoSat.Contrasena);

            var generarSolicitudService = new GenerarSolicitudService(UrlsSat.UrlSolicitud, UrlsSat.UrlSolicitudAction);

            Logger.Info("Generando XML.");
            var xml = generarSolicitudService.Generate(
                certificadoSat,
                configuracionGeneral.CertificadoSat.RfcEmisor,
                "",
                configuracionGeneral.CertificadoSat.RfcEmisor,
                solicitud.FechaInicio.ToString("yyyy-MM-dd"),
                solicitud.FechaFin.ToString("yyyy-MM-dd"));
            Logger.Info(xml);

            Logger.Info("Enviando");
            var solicitudResult = generarSolicitudService.Send(xml,solicitud.Autorizacion);
            solicitud.SolicitudSatId = solicitudResult.IdSolicitud;

            Logger.Info("Estatus {0}", solicitudResult.CodEstatus);
            Logger.Info("Solicitud Id: {0}", solicitud.SolicitudSatId);

            solicitud.Estatus = EstatusSolicitud.Generada;

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}