using System;
using System.Data.Entity;
using System.Linq;
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
            Logger.WithProperty(LogPropertyConstants.SolicitudId, request.SolicitudId).Info("Verificando solicitud {0}", request.SolicitudId);

            var configuracionGeneral = await _context.ConfiguracionGeneral.FirstAsync(cancellationToken);
            var solicitud = await _context.Solicitudes
                .Include(s => s.SolicitudAutenticacion)
                .Include(s => s.SolicitudSolicitud)
                .Include(s => s.SolicitudVerificacion)
                .Include(s => s.SolicitudesWeb)
                .SingleAsync(s => s.Id == request.SolicitudId, cancellationToken);

            if (solicitud.SolicitudAutenticacion == null)
            {
                Logger.WithProperty(LogPropertyConstants.SolicitudId, request.SolicitudId).Error("No se puede verificar la solicitud {0} por que no existe una solicitud de autenticacion.", solicitud.Id);
                throw new InvalidOperationException($"No se puede verificar la solicitud {solicitud.Id} por que no existe una solicitud de autenticacion.");
            }

            if (!solicitud.SolicitudAutenticacion.IsTokenValido)
            {
                Logger.WithProperty(LogPropertyConstants.SolicitudId, request.SolicitudId).Error("No se puede verificar la solicitud {0} por que el token no es valido.", solicitud.Id);
                throw new InvalidOperationException($"No se puede verificar la solicitud {solicitud.Id} por que el token no es valido.");
            }

            if (!solicitud.SolicitudSolicitud.IsValid)
            {
                Logger.WithProperty(LogPropertyConstants.SolicitudId, request.SolicitudId).Error("No se puede verifivar la solicitud {0} por que la solicitud SAT no es valida.", solicitud.Id);
                throw new InvalidOperationException($"No se puede verificar la solicitud {solicitud.Id} por que la solicitud del SAT no es valida.");
            }

            Logger.WithProperty(LogPropertyConstants.SolicitudId, request.SolicitudId).Info("Obteniendo certificado.");
            var certificadoSat = CertificadoService.ObtenerCertificado(configuracionGeneral.CertificadoSat.Certificado, configuracionGeneral.CertificadoSat.Contrasena);

            var verificaSolicitudService = new VerificaSolicitudService(UrlsSat.UrlVerificarSolicitud, UrlsSat.UrlVerificarSolicitudAction);

            Logger.WithProperty(LogPropertyConstants.SolicitudId, solicitud.Id).Info("Generando XML SOAP de solicitud.");
            var soapRequestEnvelopeXml = verificaSolicitudService.Generate(certificadoSat, solicitud.RfcSolicitante, solicitud.SolicitudSolicitud.IdSolicitud);
            Logger.WithProperty(LogPropertyConstants.SolicitudId, solicitud.Id).Info("SoapRequestEnvelopeXml: {0}", soapRequestEnvelopeXml);

            SolicitudResult solicitudResult;
            try
            {
                Logger.WithProperty(LogPropertyConstants.SolicitudId, solicitud.Id).Info("Enviando solicitud SOAP de verificacion.");
                solicitudResult = verificaSolicitudService.Send(soapRequestEnvelopeXml, solicitud.SolicitudAutenticacion.Autorizacion);
            }
            catch (Exception e)
            {
                Logger.WithProperty(LogPropertyConstants.SolicitudId, solicitud.Id).Error(e, "Se produjo un error al enviar la solicitud SOAP de verificacion.");

                var solicitudVerificacionError = SolicitudVerificacion.CreateInstance(
                    soapRequestEnvelopeXml,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    e.ToString());

                solicitud.SolicitudesWeb.Add(solicitudVerificacionError);
                solicitud.SolicitudVerificacion = solicitudVerificacionError;
                await _context.SaveChangesAsync(cancellationToken);
                
                throw;
            }

            var verificarSolicitudResult = solicitudResult.GetVerificarSolicitudResult();
            Logger.WithProperty(LogPropertyConstants.SolicitudId, solicitud.Id).Info("VerificarSolicitudResult: {@VerificarSolicitudResult}", verificarSolicitudResult);


            Logger.WithProperty(LogPropertyConstants.SolicitudId, solicitud.Id).Info("Creando registro de solicitud de verificacion.");
            var solicitudVerificacion = SolicitudVerificacion.CreateInstance(
                soapRequestEnvelopeXml,
                verificarSolicitudResult.response,
                verificarSolicitudResult.codEstatus,
                verificarSolicitudResult.mensaje,
                verificarSolicitudResult.codigoEstadoSolicitud,
                verificarSolicitudResult.estadoSolicitud,
                verificarSolicitudResult.numeroCfdis,
                verificarSolicitudResult.idsPaquetes.Select(idPaquete => PaqueteId.Crear(idPaquete)).ToList(),
                null);

            Logger.WithProperty(LogPropertyConstants.SolicitudId, solicitud.Id).Info("SolicitudVerificacion: {@SolicitudVerificacion}", solicitudVerificacion);

            solicitud.SolicitudesWeb.Add(solicitudVerificacion);
            solicitud.SolicitudVerificacion = solicitudVerificacion;

            Logger.WithProperty(LogPropertyConstants.SolicitudId, solicitud.Id).Info("Guardando cambios.");
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}