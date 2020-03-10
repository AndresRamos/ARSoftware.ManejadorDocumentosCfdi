using System;
using System.IO;
using System.Web;
using ARSoftware.Cfdi.DescargaMasiva.Constants;
using ARSoftware.Cfdi.DescargaMasiva.Enumerations;
using ARSoftware.Cfdi.DescargaMasiva.Helpers;
using ARSoftware.Cfdi.DescargaMasiva.Models;
using ARSoftware.Cfdi.DescargaMasiva.Services;

namespace Presentation.ConsoleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Parameters
            var certificadoPfx = new byte[0];
            var certificadoPassword = "";
            var fechaInicio = DateTime.Today;
            var fechaFin = DateTime.Today;
            var tipoSolicitud = TipoSolicitud.Cfdi;
            var rfcEmisor = "";
            var rfcReceptor = "";
            var rfcSolicitante = "";

            var certificadoSat = X509Certificate2Helper.GetCertificate(certificadoPfx, certificadoPassword);

            // Autenticacion
            var autenticacionService = new AutenticacionService(CfdiDescargaMasivaWebServiceUrls.AutenticacionUrl, CfdiDescargaMasivaWebServiceUrls.AutenticacionSoapActionUrl);
            var autenticacionRequest = AutenticacionRequest.CreateInstance();
            var soapRequestEnvelopeXml = autenticacionService.GenerateSoapRequestEnvelopeXmlContent(autenticacionRequest, certificadoSat);
            var autenticacionResult = autenticacionService.SendSoapRequest(soapRequestEnvelopeXml);
            var authorizationHttpRequestHeader = $@"WRAP access_token=""{HttpUtility.UrlDecode(autenticacionResult.Token)}""";

            // Solicitud
            var solicitudService = new SolicitudService(CfdiDescargaMasivaWebServiceUrls.SolicitudUrl, CfdiDescargaMasivaWebServiceUrls.SolicitudSoapActionUrl);
            var solicitudRequest = SolicitudRequest.CreateInstance(
                fechaInicio,
                fechaFin,
                tipoSolicitud,
                rfcEmisor,
                rfcReceptor,
                rfcSolicitante);
            soapRequestEnvelopeXml = SolicitudService.GenerateSoapRequestEnvelopeXmlContent(solicitudRequest, certificadoSat);
            var solicitudResult = solicitudService.SendSoapRequest(soapRequestEnvelopeXml, authorizationHttpRequestHeader);

            // Verificacion
            var verificaSolicitudService = new VerificacionService(CfdiDescargaMasivaWebServiceUrls.VerificacionUrl, CfdiDescargaMasivaWebServiceUrls.VerificacionSoapActionUrl);
            var verificacionRequest = VerificacionRequest.CreateInstance(solicitudResult.IdSolicitud, rfcSolicitante);
            soapRequestEnvelopeXml = verificaSolicitudService.GenerateSoapRequestEnvelopeXmlContent(verificacionRequest, certificadoSat);
            var verificacionResult = verificaSolicitudService.SendSoapRequest(soapRequestEnvelopeXml, authorizationHttpRequestHeader);

            // Descarga
            var descargarSolicitudService = new DescargaService(CfdiDescargaMasivaWebServiceUrls.DescargaUrl, CfdiDescargaMasivaWebServiceUrls.DescargaSoapActionUrl);
            foreach (var idsPaquete in verificacionResult.IdsPaquetes)
            {
                var descargaRequest = DescargaRequest.CreateInstace(idsPaquete, rfcSolicitante);
                soapRequestEnvelopeXml = descargarSolicitudService.GenerateSoapRequestEnvelopeXmlContent(descargaRequest, certificadoSat);
                var descargaResult = descargarSolicitudService.SendSoapRequest(soapRequestEnvelopeXml, authorizationHttpRequestHeader);

                var fileName = Path.Combine(@"C:\CFDIS", $"{idsPaquete}.zip");
                var paqueteContenido = Convert.FromBase64String(descargaResult.Paquete);

                using (var fileStream = File.Create(fileName, paqueteContenido.Length))
                {
                    fileStream.Write(paqueteContenido, 0, paqueteContenido.Length);
                }
            }
        }
    }
}