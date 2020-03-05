using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using Infrastructure.Sat.Models;

namespace Infrastructure.Sat.Services
{
    public class VerificaSolicitudService : SoapRequestBase
    {
        public VerificaSolicitudService(string url, string action)
            : base(url, action)
        {
        }

        public string Generate(X509Certificate2 certificate, string rfcSolicitante, string idSolicitud)
        {
            return GenerarSoapRequestEnvelopeXml(idSolicitud, rfcSolicitante, certificate);
        }

        public override SolicitudResult GetResult(string webResponse)
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(webResponse);

            if (xmlDocument.GetElementsByTagName("VerificaSolicitudDescargaResult").Count > 0)
            {
                var verificaSolicitudDescargaResultElement = xmlDocument.GetElementsByTagName("VerificaSolicitudDescargaResult")[0];
                var codigoEstadoSolicitud = verificaSolicitudDescargaResultElement.Attributes.GetNamedItem("CodigoEstadoSolicitud")?.Value;
                var estadoSolicitud = verificaSolicitudDescargaResultElement.Attributes.GetNamedItem("EstadoSolicitud")?.Value;
                var codEstatus = verificaSolicitudDescargaResultElement.Attributes.GetNamedItem("CodEstatus")?.Value;
                var numeroCfdis = verificaSolicitudDescargaResultElement.Attributes.GetNamedItem("NumeroCFDIs")?.Value;
                var mensaje = verificaSolicitudDescargaResultElement.Attributes.GetNamedItem("Mensaje")?.Value;

                var idsPaquetesList = new List<string>();

                if (estadoSolicitud == "3")
                {
                    var idsPaquetesElements = xmlDocument.GetElementsByTagName("IdsPaquetes");
                   
                    foreach (XmlNode idPaqueteElement in idsPaquetesElements)
                    {
                        idsPaquetesList.Add(idPaqueteElement.InnerText);
                    }
                }

                return SolicitudResult.CrearVerificarSolicitudResult(codEstatus, codigoEstadoSolicitud, estadoSolicitud, numeroCfdis, mensaje, idsPaquetesList, webResponse);
            }

            throw new ArgumentNullException("El resultado no contiene el nodo VerificaSolicitudDescargaResult");
        }

        public static string GenerarSoapRequestEnvelopeXml(string idSolicitud, string rfcSolicitante, X509Certificate2 certificate)
        {
            var xmlDocument = new XmlDocument();

            var envelopElement = xmlDocument.CreateElement("soapenv", "Envelope", "http://schemas.xmlsoap.org/soap/envelope/");
            envelopElement.SetAttribute("xmlns:soapenv", "http://schemas.xmlsoap.org/soap/envelope/");
            envelopElement.SetAttribute("xmlns:des", "http://DescargaMasivaTerceros.sat.gob.mx");
            envelopElement.SetAttribute("xmlns:xd", "http://www.w3.org/2000/09/xmldsig#");
            xmlDocument.AppendChild(envelopElement);

            var headerElement = xmlDocument.CreateElement("soapenv", "Header", "http://schemas.xmlsoap.org/soap/envelope/");
            envelopElement.AppendChild(headerElement);

            var bodyElement = xmlDocument.CreateElement("soapenv", "Body", "http://schemas.xmlsoap.org/soap/envelope/");
            envelopElement.AppendChild(bodyElement);

            var verificaSolicitudDescargaElement = xmlDocument.CreateElement("des", "VerificaSolicitudDescarga", "http://DescargaMasivaTerceros.sat.gob.mx");
            bodyElement.AppendChild(verificaSolicitudDescargaElement);

            var solicitudElement = xmlDocument.CreateElement("des", "solicitud", "http://DescargaMasivaTerceros.sat.gob.mx");
            solicitudElement.SetAttribute("IdSolicitud", idSolicitud);
            solicitudElement.SetAttribute("RfcSolicitante", rfcSolicitante);

            var signatureElement = FirmarXml(solicitudElement, certificate);
            solicitudElement.AppendChild(signatureElement);

            verificaSolicitudDescargaElement.AppendChild(solicitudElement);

            return xmlDocument.OuterXml;
        }
    }
}