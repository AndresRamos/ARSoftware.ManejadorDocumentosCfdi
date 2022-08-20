using System;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using ARSoftware.Cfdi.DescargaMasiva.Helpers;
using ARSoftware.Cfdi.DescargaMasiva.Interfaces;
using ARSoftware.Cfdi.DescargaMasiva.Models;
using Common;
using Core.Domain.Entities;
using Infrastructure.Persistance;
using MediatR;
using NLog;

namespace Core.Application.Solicitudes.Commands.VerificarSolicitud
{
    public class VerificarSolicitudCommandHandler : IRequestHandler<VerificarSolicitudCommand, Unit>
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ManejadorDocumentosCfdiDbContext _context;
        private readonly IVerificacionService _verificacionService;

        public VerificarSolicitudCommandHandler(ManejadorDocumentosCfdiDbContext context, IVerificacionService verificacionService)
        {
            _context = context;
            _verificacionService = verificacionService;
        }

        public async Task<Unit> Handle(VerificarSolicitudCommand request, CancellationToken cancellationToken)
        {
            Logger.WithProperty(LogPropertyConstants.SolicitudId, request.SolicitudId)
                .Info("Verificando solicitud {0}", request.SolicitudId);

            Domain.Entities.ConfiguracionGeneral configuracionGeneral = await _context.ConfiguracionGeneral.FirstAsync(cancellationToken);
            Solicitud solicitud = await _context.Solicitudes.Include(s => s.SolicitudAutenticacion)
                .Include(s => s.SolicitudSolicitud)
                .Include(s => s.SolicitudVerificacion)
                .Include(s => s.SolicitudesWeb)
                .SingleAsync(s => s.Id == request.SolicitudId, cancellationToken);

            if (solicitud.SolicitudAutenticacion == null)
            {
                Logger.WithProperty(LogPropertyConstants.SolicitudId, request.SolicitudId)
                    .Error("No se puede verificar la solicitud {0} por que no existe una solicitud de autenticacion.", solicitud.Id);
                throw new InvalidOperationException(
                    $"No se puede verificar la solicitud {solicitud.Id} por que no existe una solicitud de autenticacion.");
            }

            if (!solicitud.SolicitudAutenticacion.IsTokenValido)
            {
                Logger.WithProperty(LogPropertyConstants.SolicitudId, request.SolicitudId)
                    .Error("No se puede verificar la solicitud {0} por que el token no es valido.", solicitud.Id);
                throw new InvalidOperationException($"No se puede verificar la solicitud {solicitud.Id} por que el token no es valido.");
            }

            if (!solicitud.SolicitudSolicitud.IsValid)
            {
                Logger.WithProperty(LogPropertyConstants.SolicitudId, request.SolicitudId)
                    .Error("No se puede verifivar la solicitud {0} por que la solicitud SAT no es valida.", solicitud.Id);
                throw new InvalidOperationException(
                    $"No se puede verificar la solicitud {solicitud.Id} por que la solicitud del SAT no es valida.");
            }

            Logger.WithProperty(LogPropertyConstants.SolicitudId, request.SolicitudId).Info("Obteniendo certificado.");
            X509Certificate2 certificadoSat = X509Certificate2Helper.GetCertificate(configuracionGeneral.CertificadoSat.Certificado,
                configuracionGeneral.CertificadoSat.Contrasena);

            Logger.WithProperty(LogPropertyConstants.SolicitudId, solicitud.Id).Info("Generando XML SOAP de solicitud.");
            var verificacionRequest =
                VerificacionRequest.CreateInstance(solicitud.SolicitudSolicitud.IdSolicitud, solicitud.RfcSolicitante);
            string soapRequestEnvelopeXml = _verificacionService.GenerateSoapRequestEnvelopeXmlContent(verificacionRequest, certificadoSat);
            Logger.WithProperty(LogPropertyConstants.SolicitudId, solicitud.Id).Info("SoapRequestEnvelopeXml: {0}", soapRequestEnvelopeXml);

            VerificacionResult verificacionResult;
            try
            {
                Logger.WithProperty(LogPropertyConstants.SolicitudId, solicitud.Id).Info("Enviando solicitud SOAP de verificacion.");
                verificacionResult = await _verificacionService.SendSoapRequestAsync(soapRequestEnvelopeXml,
                    solicitud.SolicitudAutenticacion.Autorizacion,
                    cancellationToken);
            }
            catch (Exception e)
            {
                Logger.WithProperty(LogPropertyConstants.SolicitudId, solicitud.Id)
                    .Error(e, "Se produjo un error al enviar la solicitud SOAP de verificacion.");

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

            Logger.WithProperty(LogPropertyConstants.SolicitudId, solicitud.Id)
                .Info("VerificarSolicitudResult: {@VerificarSolicitudResult}", verificacionResult);

            Logger.WithProperty(LogPropertyConstants.SolicitudId, solicitud.Id).Info("Creando registro de solicitud de verificacion.");
            var solicitudVerificacion = SolicitudVerificacion.CreateInstance(soapRequestEnvelopeXml,
                verificacionResult.WebResponse,
                verificacionResult.CodEstatus,
                verificacionResult.Mensaje,
                verificacionResult.CodigoEstadoSolicitud,
                verificacionResult.EstadoSolicitud,
                verificacionResult.NumeroCfdis,
                verificacionResult.IdsPaquetes.Select(idPaquete => PaqueteId.Crear(idPaquete)).ToList(),
                null);

            Logger.WithProperty(LogPropertyConstants.SolicitudId, solicitud.Id)
                .Info("SolicitudVerificacion: {@SolicitudVerificacion}", solicitudVerificacion);

            solicitud.SolicitudesWeb.Add(solicitudVerificacion);
            solicitud.SolicitudVerificacion = solicitudVerificacion;

            Logger.WithProperty(LogPropertyConstants.SolicitudId, solicitud.Id).Info("Guardando cambios.");
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
