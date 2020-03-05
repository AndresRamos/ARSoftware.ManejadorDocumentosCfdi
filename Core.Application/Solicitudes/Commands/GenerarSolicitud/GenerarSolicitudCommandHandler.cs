using System;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Core.Domain.Entities;
using Infrastructure.Persistance;
using Infrastructure.Sat;
using Infrastructure.Sat.Models;
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
            Logger.WithProperty(LogPropertyConstants.SolicitudId, request.SolicitudId).Info("Generando solicitud {0}", request.SolicitudId);

            var configuracionGeneral = await _context.ConfiguracionGeneral.FirstAsync(cancellationToken);
            var solicitud = await _context.Solicitudes
                .Include(s => s.SolicitudAutenticacion)
                .Include(s => s.SolicitudSolicitud)
                .Include(s => s.SolicitudesWeb)
                .SingleAsync(s => s.Id == request.SolicitudId, cancellationToken);

            if (solicitud.SolicitudAutenticacion == null)
            {
                Logger.WithProperty(LogPropertyConstants.SolicitudId, request.SolicitudId).Error("No se puede generar la solicitud {0} por que no existe una solicitud de autenticacion.", solicitud.Id);
                throw new InvalidOperationException($"No se puede generar la solicitud {solicitud.Id} por que no existe una solicitud de autenticacion.");
            }

            if (!solicitud.SolicitudAutenticacion.IsTokenValido)
            {
                Logger.WithProperty(LogPropertyConstants.SolicitudId, request.SolicitudId).Error("No se puede generar la solicitud {0} por que el token no es valido.", solicitud.Id);
                throw new InvalidOperationException($"No se puede generar la solicitud {solicitud.Id} por que el token no es valido.");
            }

            Logger.WithProperty(LogPropertyConstants.SolicitudId, request.SolicitudId).Info("Obteniendo certificado.");
            var certificadoSat = CertificadoService.ObtenerCertificado(configuracionGeneral.CertificadoSat.Certificado, configuracionGeneral.CertificadoSat.Contrasena);

            var generarSolicitudService = new GenerarSolicitudService(UrlsSat.UrlSolicitud, UrlsSat.UrlSolicitudAction);

            Logger.WithProperty(LogPropertyConstants.SolicitudId, solicitud.Id).Info("Generando XML SOAP de solicitud.");
            var soapRequestEnvelopeXml = GenerarSolicitudService.GenerarSoapRequestXml(
                solicitud.FechaInicio.ToString("yyyy-MM-dd") + "T00:00:00",
                solicitud.FechaFin.ToString("yyyy-MM-dd") + "T23:59:59",
                solicitud.TipoSolicitud,
                solicitud.RfcEmisor,
                solicitud.RfcReceptor,
                solicitud.RfcSolicitante,
                certificadoSat);
            Logger.WithProperty(LogPropertyConstants.SolicitudId, solicitud.Id).Info("SoapRequestEnvelopeXml: {0}", soapRequestEnvelopeXml);

            SolicitudResult solicitudResult;
            try
            {
                Logger.WithProperty(LogPropertyConstants.SolicitudId, solicitud.Id).Info("Enviando solicitud SOAP de generacion.");
                solicitudResult = generarSolicitudService.Send(soapRequestEnvelopeXml, solicitud.SolicitudAutenticacion.Autorizacion);
            }
            catch (Exception e)
            {
                Logger.WithProperty(LogPropertyConstants.SolicitudId, solicitud.Id).Error(e, "Se produjo un error al enviar la solicitud SOAP de generacion.");

                var solicitudSolicitudError = SolicitudSolicitud.CreateInstance(
                    soapRequestEnvelopeXml,
                    null,
                    solicitud.FechaInicio,
                    solicitud.FechaFin,
                    solicitud.RfcEmisor,
                    solicitud.RfcReceptor,
                    solicitud.RfcSolicitante,
                    solicitud.TipoSolicitud,
                    null,
                    null,
                    null,
                    e.ToString());

                solicitud.SolicitudesWeb.Add(solicitudSolicitudError);
                solicitud.SolicitudSolicitud = solicitudSolicitudError;
                await _context.SaveChangesAsync(cancellationToken);

                throw;
            }

            var generarSolicitudResult = solicitudResult.GetGenerarSolicitudResult();
            Logger.WithProperty(LogPropertyConstants.SolicitudId, solicitud.Id).Info("GenerarSolicitudResult: {@GenerarSolicitudResult}", generarSolicitudResult);

            Logger.WithProperty(LogPropertyConstants.SolicitudId, solicitud.Id).Info("Creando registro de solicitud de solicitud.");
            var solicitudSolicitud = SolicitudSolicitud.CreateInstance(
                soapRequestEnvelopeXml,
                generarSolicitudResult.response,
                solicitud.FechaInicio,
                solicitud.FechaFin,
                solicitud.RfcEmisor,
                solicitud.RfcReceptor,
                solicitud.RfcSolicitante,
                solicitud.TipoSolicitud,
                generarSolicitudResult.codEstatus,
                generarSolicitudResult.mensaje,
                generarSolicitudResult.idSolicitud,
                null);

            Logger.WithProperty(LogPropertyConstants.SolicitudId, solicitud.Id).Info("SolicitudSolicitud: {@SolicitudSolicitud}", solicitudSolicitud);

            solicitud.SolicitudesWeb.Add(solicitudSolicitud);
            solicitud.SolicitudSolicitud = solicitudSolicitud;

            Logger.WithProperty(LogPropertyConstants.SolicitudId, solicitud.Id).Info("Guardando cambios.");
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}