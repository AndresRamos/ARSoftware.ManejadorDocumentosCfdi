using System;
using System.Data.Entity;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Core.Application.Paquetes.Commands.ExportarArchivoZip;
using Core.Domain.Entities;
using Infrastructure.Persistance;
using ARSoftware.Cfdi.DescargaMasiva.Constants;
using ARSoftware.Cfdi.DescargaMasiva.Helpers;
using ARSoftware.Cfdi.DescargaMasiva.Models;
using ARSoftware.Cfdi.DescargaMasiva.Services;
using MediatR;
using NLog;

namespace Core.Application.Solicitudes.Commands.DescargarSolicitud
{
    public class DescargarSolicitudCommandHandler : IRequestHandler<DescargarSolicitudCommand, Unit>
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ManejadorDocumentosCfdiDbContext _context;
        private readonly IMediator _mediator;

        public DescargarSolicitudCommandHandler(ManejadorDocumentosCfdiDbContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task<Unit> Handle(DescargarSolicitudCommand request, CancellationToken cancellationToken)
        {
            Logger.WithProperty(LogPropertyConstants.SolicitudId, request.SolicitudId)
                .Info("Descargando solicitud {0}", request.SolicitudId);

            var configuracionGeneral = await _context.ConfiguracionGeneral.FirstAsync(cancellationToken);
            var solicitud = await _context.Solicitudes
                .Include(s => s.SolicitudAutenticacion)
                .Include(s => s.SolicitudSolicitud)
                .Include(s => s.SolicitudVerificacion.PaquetesIds)
                .Include(s => s.SolicitudesWeb)
                .Include(s => s.Paquetes)
                .SingleAsync(s => s.Id == request.SolicitudId, cancellationToken);

            if (solicitud.SolicitudAutenticacion == null)
            {
                Logger.WithProperty(LogPropertyConstants.SolicitudId, request.SolicitudId).Error("No se puede descargar la solicitud {0} por que no existe una solicitud de autenticacion.", solicitud.Id);
                throw new InvalidOperationException($"No se puede descargar la solicitud {solicitud.Id} por que no existe una solicitud de autenticacion.");
            }

            if (!solicitud.SolicitudAutenticacion.IsTokenValido)
            {
                Logger.WithProperty(LogPropertyConstants.SolicitudId, solicitud.Id).Error("La solicitud {0} no se puede descargar por que no tiene un token valido.", solicitud.Id);
                throw new InvalidOperationException($"La solicitud {solicitud.Id} no se puede descargar por que no tiene un token valido.");
            }

            if (!solicitud.SolicitudSolicitud.IsValid)
            {
                Logger.WithProperty(LogPropertyConstants.SolicitudId, request.SolicitudId).Error("No se puede descargar la solicitud {0} por que la solicitud SAT no es valida.", solicitud.Id);
                throw new InvalidOperationException($"No se puede descargar la solicitud {solicitud.Id} por que la solicitud SAT no es valida.");
            }

            if (!solicitud.SolicitudVerificacion.IsValid)
            {
                Logger.WithProperty(LogPropertyConstants.SolicitudId, solicitud.Id)
                    .Error("La solicitud {0} no se puede descargar por que no tiene una solicitud de verificacion valida.", solicitud.Id);
                throw new InvalidOperationException($"La solicitud {solicitud.Id} no se puede descargar por que no tiene una solicitud de verificacion valida.");
            }

            Logger.WithProperty(LogPropertyConstants.SolicitudId, solicitud.Id)
                .Info("Obteniendo certificado.");
            var certificadoSat = X509Certificate2Helper.GetCertificate(configuracionGeneral.CertificadoSat.Certificado, configuracionGeneral.CertificadoSat.Contrasena);

            var descargarSolicitudService = new DescargaService(CfdiDescargaMasivaWebServiceUrls.DescargaUrl, CfdiDescargaMasivaWebServiceUrls.DescargaSoapActionUrl);

            Logger.WithProperty(LogPropertyConstants.SolicitudId, solicitud.Id)
                .Info("# de paquetes a descargar = {0}", solicitud.SolicitudVerificacion.PaquetesIds.Count);
            foreach (var paqueteId in solicitud.SolicitudVerificacion.PaquetesIds)
            {
                Logger.WithProperty(LogPropertyConstants.SolicitudId, solicitud.Id).Info("Descargando paquete {0}", paqueteId.IdPaquete);
                Logger.WithProperty(LogPropertyConstants.SolicitudId, solicitud.Id).Info("Generando XML SOAP de solicitud.");
                var descargaRequest = DescargaRequest.CreateInstace(paqueteId.IdPaquete, solicitud.SolicitudSolicitud.RfcSolicitante);
                var soapRequestEnvelopeXml = descargarSolicitudService.GenerateSoapRequestEnvelopeXmlContent(descargaRequest, certificadoSat);
                Logger.WithProperty(LogPropertyConstants.SolicitudId, solicitud.Id).Info("SoapRequestEnvelopeXml: {0}", soapRequestEnvelopeXml);

                DescargaResult descargaResult;
                try
                {
                    Logger.WithProperty(LogPropertyConstants.SolicitudId, solicitud.Id).Info("Enviando solicitud SOAP.");
                    descargaResult = descargarSolicitudService.SendSoapRequest(soapRequestEnvelopeXml, solicitud.SolicitudAutenticacion.Autorizacion);
                }
                catch (Exception e)
                {
                    Logger.WithProperty(LogPropertyConstants.SolicitudId, solicitud.Id).Error(e, "Se produjo un error al enviar la solicitud SOAP.");

                    var solicitudDescargaError = SolicitudDescarga.CreateInstance(
                        soapRequestEnvelopeXml,
                        null,
                        null,
                        null,
                        paqueteId.IdPaquete,
                        null,
                        e.ToString());

                    solicitud.SolicitudesWeb.Add(solicitudDescargaError);
                    solicitud.SolicitudDescarga = solicitudDescargaError;
                    await _context.SaveChangesAsync(cancellationToken);

                    throw;
                }

                Logger.WithProperty(LogPropertyConstants.SolicitudId, solicitud.Id).Info("DescargaSolicitudResult: {@DescargaSolicitudResult}", descargaResult);

                var solicitudDescarga = SolicitudDescarga.CreateInstance(
                    soapRequestEnvelopeXml,
                    descargaResult.WebResponse,
                    descargaResult.CodEstatus,
                    descargaResult.Mensaje,
                    paqueteId.IdPaquete,
                    descargaResult.Paquete,
                    null);

                solicitud.SolicitudesWeb.Add(solicitudDescarga);
                solicitud.SolicitudDescarga = solicitudDescarga;
                paqueteId.SetDescargado();
                solicitud.Paquetes.Add(Paquete.Crear(solicitudDescarga.PaqueteId, Convert.FromBase64String(solicitudDescarga.Paquete)));
                Logger.WithProperty(LogPropertyConstants.SolicitudId, solicitud.Id).Info("Guardando cambios.");
                await _context.SaveChangesAsync(cancellationToken);
            }

            foreach (var paquete in solicitud.Paquetes)
            {
                Logger.WithProperty(LogPropertyConstants.SolicitudId, solicitud.Id).Info("Creando directorio de desscarga.");
                Directory.CreateDirectory(configuracionGeneral.RutaDirectorioDescargas);

                Logger.WithProperty(LogPropertyConstants.SolicitudId, solicitud.Id).Info("Creando archivo .zip");
                await _mediator.Send(new ExportarArchivoZipCommand(paquete.Id, Path.Combine(configuracionGeneral.RutaDirectorioDescargas, $"{paquete.IdSat}.zip")), cancellationToken);

                //todo Descomprimir
            }

            return Unit.Value;
        }
    }
}