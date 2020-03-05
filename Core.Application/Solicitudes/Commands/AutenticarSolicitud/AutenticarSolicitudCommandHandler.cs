using System;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Common;
using Core.Domain.Entities;
using Infrastructure.Persistance;
using Infrastructure.Sat;
using Infrastructure.Sat.Models;
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
            Logger.WithProperty(LogPropertyConstants.SolicitudId, request.SolicitudId).Info("Autenticando solicitud {0}.", request.SolicitudId);

            var configuracionGeneral = await _context.ConfiguracionGeneral.FirstAsync(cancellationToken);
            var solicitud = await _context.Solicitudes
                .Include(s => s.SolicitudAutenticacion)
                .Include(s => s.SolicitudesWeb)
                .SingleAsync(s => s.Id == request.SolicitudId, cancellationToken);

            if (solicitud.SolicitudAutenticacion != null && solicitud.SolicitudAutenticacion.IsTokenValido)
            {
                Logger.WithProperty(LogPropertyConstants.SolicitudId, request.SolicitudId).Info("No se puede autenticar la solicitud {0} por que el token sigue siendo valido.", solicitud.Id);
                throw new InvalidOperationException($"No se puede autenticar la solicitud {solicitud.Id} por que el token sigue siendo valido.");
            }

            Logger.WithProperty(LogPropertyConstants.SolicitudId, request.SolicitudId).Info("Obteniendo certificado.");
            var certificadoSat = CertificadoService.ObtenerCertificado(configuracionGeneral.CertificadoSat.Certificado, configuracionGeneral.CertificadoSat.Contrasena);

            var autenticacionService = new AutenticacionService(UrlsSat.UrlAutentica, UrlsSat.UrlAutenticaAction);

            var fechaTokenCreacionUtc = DateTime.UtcNow;
            var fechaTokenExpiracionUtc = fechaTokenCreacionUtc.AddMinutes(5);

            Logger.WithProperty(LogPropertyConstants.SolicitudId, solicitud.Id).Info("Generando XML SOAP de solicitud.");
            var soapRequestEnvelopeXml = autenticacionService.Generate(certificadoSat, fechaTokenCreacionUtc, fechaTokenExpiracionUtc);
            Logger.WithProperty(LogPropertyConstants.SolicitudId, solicitud.Id).Info("SoapRequestEnvelopeXml: {0}", soapRequestEnvelopeXml);

            SolicitudResult solicitudResult;
            try
            {
                Logger.WithProperty(LogPropertyConstants.SolicitudId, solicitud.Id).Info("Enviando solicitud SOAP de autenticacion.");
                solicitudResult = autenticacionService.Send(soapRequestEnvelopeXml, null);
            }
            catch (Exception e)
            {
                Logger.WithProperty(LogPropertyConstants.SolicitudId, solicitud.Id).Error(e, "Se produjo un error al enviar la solicitud SOAP de autenticacion.");

                var solicitudAutenticacionError = SolicitudAutenticacion.CreateInstance(
                    soapRequestEnvelopeXml,
                    null,
                    fechaTokenCreacionUtc,
                    fechaTokenExpiracionUtc,
                    null,
                    null,
                    null,
                    null,
                    e.ToString());

                solicitud.SolicitudesWeb.Add(solicitudAutenticacionError);
                solicitud.SolicitudAutenticacion = solicitudAutenticacionError;
                await _context.SaveChangesAsync(cancellationToken);

                throw;
            }

            var autenticaResult = solicitudResult.GetAutenticaResult();
            Logger.WithProperty(LogPropertyConstants.SolicitudId, solicitud.Id).Info("AutenticaResult: {@AutenticaResult}", autenticaResult);

            Logger.WithProperty(LogPropertyConstants.SolicitudId, solicitud.Id).Info("Creando registro de solicitud de autenticacion.");
            var solicitudAutenticacion = SolicitudAutenticacion.CreateInstance(
                soapRequestEnvelopeXml,
                autenticaResult.response,
                fechaTokenCreacionUtc,
                fechaTokenExpiracionUtc,
                autenticaResult.token,
                autenticaResult.token != null ? $"WRAP access_token=\"{HttpUtility.UrlDecode(autenticaResult.token)}\"" : null,
                autenticaResult.faultCode,
                autenticaResult.faultString,
                null);

            Logger.WithProperty(LogPropertyConstants.SolicitudId, solicitud.Id).Info("SolicitudAutenticacion: {@SolicitudAutenticacion}", solicitudAutenticacion);

            solicitud.SolicitudesWeb.Add(solicitudAutenticacion);
            solicitud.SolicitudAutenticacion = solicitudAutenticacion;

            Logger.WithProperty(LogPropertyConstants.SolicitudId, solicitud.Id).Info("Guardando cambios.");
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}