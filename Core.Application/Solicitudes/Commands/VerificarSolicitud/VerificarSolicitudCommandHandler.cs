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

namespace Core.Application.Solicitudes.Commands.VerificarSolicitud
{
    public class VerificarSolicitudCommandHandler : IRequestHandler<VerificarSolicitudCommand, Unit>
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ManejadorDocumentosCfdiDbContext _context;

        public VerificarSolicitudCommandHandler(ManejadorDocumentosCfdiDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(VerificarSolicitudCommand request, CancellationToken cancellationToken)
        {
            Logger.Info("Verificando solicitud {0}", request.SolicitudId);

            var configuracionGeneral = await _context.ConfiguracionGeneral.FirstAsync(cancellationToken);
            var solicitud = await _context.Solicitudes.SingleAsync(s => s.Id == request.SolicitudId, cancellationToken);

            //if (solicitud.Estatus != EstatusSolicitud.Generada)
            //{
            //    throw new InvalidOperationException("La solicitud no esta generada.");
            //}

            Logger.Info("Obteniendo certificado.");
            var certificadoSat = CertificadoService.ObtenerCertificado(configuracionGeneral.CertificadoSat.Certificado, configuracionGeneral.CertificadoSat.Contrasena);

            var verifica = new VerificaSolicitudService(UrlsSat.UrlVerificarSolicitud, UrlsSat.UrlVerificarSolicitudAction);
            Logger.Info("Generando XML.");
            var xml = verifica.Generate(certificadoSat, configuracionGeneral.CertificadoSat.RfcEmisor, solicitud.SolicitudSatId);
            Logger.Info(xml);

            Logger.Info("Enviando");

            var solicitudResult = verifica.Send(xml,solicitud.Autorizacion);

            Logger.Info("CodigoEstadoSolicitud {0}", solicitudResult.CodigoEstadoSolicitud);
            Logger.Info("EstadoSolicitud {0}", solicitudResult.EstadoSolicitud);
            Logger.Info("IdsPaquetes {0}", solicitudResult.IdsPaquetes);

            while (solicitudResult.EstadoSolicitud == "1" || solicitudResult.EstadoSolicitud == "2")
            {
                await Task.Delay(10000, cancellationToken);

                Logger.Info("Enviando");
                solicitudResult = verifica.Send(xml, solicitud.Autorizacion);
                Logger.Info("CodigoEstadoSolicitud {0}", solicitudResult.CodigoEstadoSolicitud);
                Logger.Info("EstadoSolicitud {0}", solicitudResult.EstadoSolicitud);
                Logger.Info("IdsPaquetes {0}", solicitudResult.IdsPaquetes);
            }

            solicitud.PaqueteId = solicitudResult.IdsPaquetes;

            Logger.Info("Paquete Id: {0}", solicitud.PaqueteId);

            solicitud.Estatus = EstatusSolicitud.Verificada;

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}