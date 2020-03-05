using System;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using Infrastructure.Sat.Models;

namespace Infrastructure.Sat.Services
{
    public class DescargarSolicitudService : SoapRequestBase
    {
        public DescargarSolicitudService(string url, string action)
            : base(url, action)
        {
        }

        #region Crea el XML para enviar

        public string Generate(X509Certificate2 certificate, string rfcSolicitante, string idPaquete)
        {
            var canonicalTimestamp = "<des:PeticionDescargaMasivaTercerosEntrada xmlns:des=\"http://DescargaMasivaTerceros.sat.gob.mx\">"
                                     + "<des:peticionDescarga IdPaquete=\"" + idPaquete + "\" RfcSolicitante=\"" + rfcSolicitante + ">"
                                     + "</des:peticionDescarga>"
                                     + "</des:PeticionDescargaMasivaTercerosEntrada>";

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
                               @"<des:PeticionDescargaMasivaTercerosEntrada>" +
                               @"<des:peticionDescarga " +
                               @"IdPaquete=""" + idPaquete +
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
                               @"</des:peticionDescarga>" +
                               @"</des:PeticionDescargaMasivaTercerosEntrada>" +
                               @"</s:Body>" +
                               @"</s:Envelope>";
            return soap_request;
        }

        #endregion

        public override SolicitudResult GetResult(string webResponse)
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(webResponse);

            var element = xmlDocument.GetElementsByTagName("h:respuesta")[0];
            var codEstatus = element.Attributes.GetNamedItem("CodEstatus").Value;
            var mensaje = element.Attributes.GetNamedItem("Mensaje").Value;
            var paqete = xmlDocument.GetElementsByTagName("Paquete")[0].InnerXml;

            return SolicitudResult.CrearDescargaSolicitudResult(codEstatus, mensaje, paqete, webResponse);

            //var s = xmlDoc.GetElementsByTagName("Paquete")[0].InnerXml;
            //return s;
        }

        public string GenerarSoapRequestEnvelopeXml(string idPaquete, string rfcSolicitante, X509Certificate2 certificate)
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

            var peticionDescargaMasivaTercerosEntradaElement = xmlDocument.CreateElement("des", "PeticionDescargaMasivaTercerosEntrada", "http://DescargaMasivaTerceros.sat.gob.mx");
            bodyElement.AppendChild(peticionDescargaMasivaTercerosEntradaElement);

            var peticionDescargaElement = xmlDocument.CreateElement("des", "peticionDescarga", "http://DescargaMasivaTerceros.sat.gob.mx");
            peticionDescargaElement.SetAttribute("IdPaquete", idPaquete);
            peticionDescargaElement.SetAttribute("RfcSolicitante", rfcSolicitante);

            var signatureElement = FirmarXml(peticionDescargaElement, certificate);
            peticionDescargaElement.AppendChild(signatureElement);

            peticionDescargaMasivaTercerosEntradaElement.AppendChild(peticionDescargaElement);

            return xmlDocument.OuterXml;
        }
    }
}