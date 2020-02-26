using System;
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

        #region Crea el XML para enviar

        public string Generate(X509Certificate2 certificate, string rfcSolicitante, string idSolicitud)
        {
            var canonicalTimestamp = "<des:VerificaSolicitudDescarga xmlns:des=\"http://DescargaMasivaTerceros.sat.gob.mx\">"
                                     + "<des:solicitud IdSolicitud=\"" + idSolicitud + "\" RfcSolicitante=\"" + rfcSolicitante + ">"
                                     + "</des:solicitud>"
                                     + "</des:VerificaSolicitudDescarga>";

            var digest = CreateDigest(canonicalTimestamp);

            var canonicalSignedInfo = @"<SignedInfo xmlns=""http://www.w3.org/2000/09/xmldsig#"">" +
                                      @"<CanonicalizationMethod Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#""></CanonicalizationMethod>" +
                                      @"<SignatureMethod Algorithm=""http://www.w3.org/2000/09/xmldsig#rsa-sha1""></SignatureMethod>" +
                                      @"<Reference URI=""#_0"">" +
                                      "<Transforms>" +
                                      @"<Transform Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#""></Transform>" +
                                      "</Transforms>" +
                                      @"<DigestMethod Algorithm=""http://www.w3.org/2000/09/xmldsig#sha1""></DigestMethod>" +
                                      "<DigestValue>" + digest + "</DigestValue>" +
                                      "</Reference>" +
                                      "</SignedInfo>";
            var signature = Sign(canonicalSignedInfo, certificate);
            var soap_request = @"<s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:u=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd"" xmlns:des=""http://DescargaMasivaTerceros.sat.gob.mx"" xmlns:xd=""http://www.w3.org/2000/09/xmldsig#"">" +
                               @"<s:Header/>" +
                               @"<s:Body>" +
                               @"<des:VerificaSolicitudDescarga>" +
                               @"<des:solicitud " +
                               @"IdSolicitud=""" + idSolicitud +
                               @""" RfcSolicitante=""" + rfcSolicitante +
                               @""">" +
                               @"<Signature xmlns=""http://www.w3.org/2000/09/xmldsig#"">" +
                               @"<SignedInfo>" +
                               @"<CanonicalizationMethod Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#""/>" +
                               @"<SignatureMethod Algorithm=""http://www.w3.org/2000/09/xmldsig#rsa-sha1""/>" +
                               @"<Reference URI=""#_0"">" +
                               @"<Transforms>" +
                               @"<Transform Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#""/>" +
                               @"</Transforms>" +
                               @"<DigestMethod Algorithm=""http://www.w3.org/2000/09/xmldsig#sha1""/>" +
                               @"<DigestValue>" + digest + @"</DigestValue>" +
                               @"</Reference>" +
                               @"</SignedInfo>" +
                               @"<SignatureValue>" + signature + "</SignatureValue>" +
                               @"<KeyInfo>" +
                               @"<X509Data>" +
                               @"<X509IssuerSerial>" +
                               @"<X509IssuerName>" + certificate.Issuer +
                               @"</X509IssuerName>" +
                               @"<X509SerialNumber>" + certificate.SerialNumber +
                               @"</X509SerialNumber>" +
                               @"</X509IssuerSerial>" +
                               @"<X509Certificate>" + Convert.ToBase64String(certificate.RawData) + "</X509Certificate>" +
                               @"</X509Data>" +
                               @"</KeyInfo>" +
                               @"</Signature>" +
                               @"</des:solicitud>" +
                               @"</des:VerificaSolicitudDescarga>" +
                               @"</s:Body>" +
                               @"</s:Envelope>";
            return soap_request;
        }

        #endregion

        public override SolicitudResult GetResult(string webResponse)
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(webResponse);

            if (xmlDocument.GetElementsByTagName("VerificaSolicitudDescargaResult").Count > 0)
            {
                var verificaXml = xmlDocument.GetElementsByTagName("VerificaSolicitudDescargaResult")[0];
                var codigoEstadoSolicitud = verificaXml.Attributes.GetNamedItem("CodigoEstadoSolicitud")?.Value;
                var estadoSolicitud = verificaXml.Attributes.GetNamedItem("EstadoSolicitud")?.Value;
                var codEstatus = verificaXml.Attributes.GetNamedItem("CodEstatus")?.Value;
                var numeroCfdis = verificaXml.Attributes.GetNamedItem("NumeroCFDIs")?.Value;
                var mensaje = verificaXml.Attributes.GetNamedItem("Mensaje")?.Value;
                string idsPaquetes = string.Empty;

                if (estadoSolicitud == "3")
                {
                    idsPaquetes = xmlDocument.GetElementsByTagName("IdsPaquetes")[0]?.InnerXml;
                }

                return SolicitudResult.CrearVerificarSolicitudResult(codEstatus, codigoEstadoSolicitud, estadoSolicitud, numeroCfdis, mensaje, idsPaquetes, webResponse);
            }

            throw new ArgumentNullException($"El resultado no contiene el nodo VerificaSolicitudDescargaResult");
        }
    }
}