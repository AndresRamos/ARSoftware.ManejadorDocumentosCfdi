using System;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using ARSoftware.Cfdi.DescargaMasiva.Constants;
using ARSoftware.Cfdi.DescargaMasiva.Helpers;
using ARSoftware.Cfdi.DescargaMasiva.Models;
using ARSoftware.Cfdi.DescargaMasiva.Services;
using Common;
using Core.Domain.Entities;
using Infrastructure.Persistance;
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
            var certificadoSat = X509Certificate2Helper.GetCertificate(configuracionGeneral.CertificadoSat.Certificado, configuracionGeneral.CertificadoSat.Contrasena);

            var autenticacionService = new AutenticacionService(CfdiDescargaMasivaWebServiceUrls.AutenticacionUrl, CfdiDescargaMasivaWebServiceUrls.AutenticacionSoapActionUrl);

            Logger.WithProperty(LogPropertyConstants.SolicitudId, solicitud.Id).Info("Generando XML SOAP de solicitud.");
            var autenticacionRequest = AutenticacionRequest.CreateInstance();
            var soapRequestEnvelopeXml = autenticacionService.GenerateSoapRequestEnvelopeXmlContent(autenticacionRequest, certificadoSat);
            Logger.WithProperty(LogPropertyConstants.SolicitudId, solicitud.Id).Info("SoapRequestEnvelopeXml: {0}", soapRequestEnvelopeXml);

            AutenticacionResult autenticacionResult;
            try
            {
                Logger.WithProperty(LogPropertyConstants.SolicitudId, solicitud.Id).Info("Enviando solicitud SOAP de autenticacion.");
                autenticacionResult = autenticacionService.SendSoapRequest(soapRequestEnvelopeXml);
            }
            catch (Exception e)
            {
                Logger.WithProperty(LogPropertyConstants.SolicitudId, solicitud.Id).Error(e, "Se produjo un error al enviar la solicitud SOAP de autenticacion.");

                var solicitudAutenticacionError = SolicitudAutenticacion.CreateInstance(
                    soapRequestEnvelopeXml,
                    null,
                    autenticacionRequest.TokenCreatedDateUtc,
                    autenticacionRequest.TokenExpiresDateUtc,
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

            Logger.WithProperty(LogPropertyConstants.SolicitudId, solicitud.Id).Info("AutenticaResult: {@AutenticaResult}", autenticacionResult);

            Logger.WithProperty(LogPropertyConstants.SolicitudId, solicitud.Id).Info("Creando registro de solicitud de autenticacion.");
            var solicitudAutenticacion = SolicitudAutenticacion.CreateInstance(
                soapRequestEnvelopeXml,
                autenticacionResult.WebResponse,
                autenticacionRequest.TokenCreatedDateUtc,
                autenticacionRequest.TokenExpiresDateUtc,
                autenticacionResult.Token,
                autenticacionResult.Token != null ? $"WRAP access_token=\"{HttpUtility.UrlDecode(autenticacionResult.Token)}\"" : null,
                autenticacionResult.FaultCode,
                autenticacionResult.FaultString,
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