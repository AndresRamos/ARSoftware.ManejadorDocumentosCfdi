using System;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using Infrastructure.Sat.Models;

namespace Infrastructure.Sat.Services
{
    public class AutenticacionService : SoapRequestBase
    {
        public AutenticacionService(string url, string action)
            : base(url, action)
        {
        }

        //public string Generate(X509Certificate2 certificate, DateTime fechaCreacion, DateTime fechaExpiracion)
        //{
        //    var format = "yyyy-MM-ddTHH:mm:ss.fffZ";
        //    var created = fechaCreacion.ToString(format);
        //    var expires = fechaExpiracion.ToString(format);
        //    var uuid = "uuid-" + Guid.NewGuid() + "-1";

        //    var canonicalTimestamp = @"<u:Timestamp xmlns:u="NamespaceConstants.wsu_Namespace" u:Id=""_0"">" +
        //                             "<u:Created>" + created + "</u:Created>" +
        //                             "<u:Expires>" + expires + "</u:Expires>" +
        //                             "</u:Timestamp>";
        //    var digest = CreateDigest(canonicalTimestamp);
        //    var canonicalSignedInfo = @"<SignedInfo xmlns=""http://www.w3.org/2000/09/xmldsig#"">" +
        //                              @"<CanonicalizationMethod Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#""></CanonicalizationMethod>" +
        //                              @"<SignatureMethod Algorithm=""http://www.w3.org/2000/09/xmldsig#rsa-sha1""></SignatureMethod>" +
        //                              @"<Reference URI=""#_0"">" +
        //                              "<Transforms>" +
        //                              @"<Transform Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#""></Transform>" +
        //                              "</Transforms>" +
        //                              @"<DigestMethod Algorithm=""http://www.w3.org/2000/09/xmldsig#sha1""></DigestMethod>" +
        //                              "<DigestValue>" + digest + "</DigestValue>" +
        //                              "</Reference>" +
        //                              "</SignedInfo>";
        //    var signature = Sign(canonicalSignedInfo, certificate);
        //    var soap_request = @"<s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:u="NamespaceConstants.wsu_Namespace">" +
        //                       @"<s:Header>" +
        //                       @"<o:Security s:mustUnderstand=""1"" xmlns:o="NamespaceConstants.wsse_Namespace">" +
        //                       @"<u:Timestamp u:Id=""_0"">" +
        //                       @"<u:Created>" + created + "</u:Created>" +
        //                       @"<u:Expires>" + expires + "</u:Expires>" +
        //                       @"</u:Timestamp>" +
        //                       @"<o:BinarySecurityToken u:Id=""" + uuid + @""" ValueType=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-x509-token-profile-1.0#X509v3"" EncodingType=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-soap-message-security-1.0#Base64Binary"">" +
        //                       Convert.ToBase64String(certificate.RawData) +
        //                       @"</o:BinarySecurityToken>" +
        //                       @"<Signature xmlns=""http://www.w3.org/2000/09/xmldsig#"">" +
        //                       @"<SignedInfo>" +
        //                       @"<CanonicalizationMethod Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#""/>" +
        //                       @"<SignatureMethod Algorithm=""http://www.w3.org/2000/09/xmldsig#rsa-sha1""/>" +
        //                       @"<Reference URI=""#_0"">" +
        //                       @"<Transforms>" +
        //                       @"<Transform Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#""/>" +
        //                       @"</Transforms>" +
        //                       @"<DigestMethod Algorithm=""http://www.w3.org/2000/09/xmldsig#sha1""/>" +
        //                       @"<DigestValue>" + digest + @"</DigestValue>" +
        //                       @"</Reference>" +
        //                       @"</SignedInfo>" +
        //                       @"<SignatureValue>" + signature + "</SignatureValue>" +
        //                       @"<KeyInfo>" +
        //                       @"<o:SecurityTokenReference>" +
        //                       @"<o:Reference ValueType=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-x509-token-profile-1.0#X509v3"" URI=""#" + uuid + @"""/>" +
        //                       @"</o:SecurityTokenReference>" +
        //                       @"</KeyInfo>" +
        //                       @"</Signature>" +
        //                       @"</o:Security>" +
        //                       @"</s:Header>" +
        //                       @"<s:Body>" +
        //                       @"<Autentica xmlns=""http://DescargaMasivaTerceros.gob.mx""/>" +
        //                       @"</s:Body>" +
        //                       @"</s:Envelope>";
        //    return soap_request;
        //}

        public override SolicitudResult GetResult(string webResponse)
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(webResponse);

            var validElement = xmlDocument.GetElementsByTagName("AutenticaResult")[0];
            if (validElement != null)
            {
                var token = xmlDocument.GetElementsByTagName("AutenticaResult")[0].InnerXml;
                return SolicitudResult.CrearAutenticaResult(token, null, null, webResponse);
            }

            var errorElement = xmlDocument.GetElementsByTagName("s:Fault")[0];
            if (errorElement != null)
            {
                var faultCode = xmlDocument.GetElementsByTagName("faultcode")[0].InnerXml;
                var faultString = xmlDocument.GetElementsByTagName("faultstring")[0].InnerXml;
                return SolicitudResult.CrearAutenticaResult(null, faultCode, faultString, webResponse);
            }

            throw new ArgumentException("La respuesta no es valida", nameof(webResponse));
        }

        public static string GenerarSoapRequestXml(X509Certificate2 certificate, string fechaCreacion, string fechaExpiracion, string uuid)
        {
            var xmlDocument = new XmlDocument();

            var envelopElement = xmlDocument.CreateElement(NamespaceConstants.S11_Prefix, "Envelope", NamespaceConstants.S11_Namespace);
            envelopElement.SetAttribute($"xmlns:{NamespaceConstants.S11_Prefix}", NamespaceConstants.S11_Namespace);
            envelopElement.SetAttribute($"xmlns:{NamespaceConstants.wsu_Prefix}", NamespaceConstants.wsu_Namespace);
            xmlDocument.AppendChild(envelopElement);

            var headerElement = xmlDocument.CreateElement(NamespaceConstants.S11_Prefix, "Header", NamespaceConstants.S11_Namespace);
            envelopElement.AppendChild(headerElement);

            var securityElement = xmlDocument.CreateElement(NamespaceConstants.wsse_Prefix, "Security", NamespaceConstants.wsse_Namespace);
            securityElement.SetAttribute("mustUnderstand", NamespaceConstants.S11_Namespace, "1");
            headerElement.AppendChild(securityElement);

            var timestampElement = xmlDocument.CreateElement(NamespaceConstants.wsu_Prefix, "Timestamp", NamespaceConstants.wsu_Namespace);
            timestampElement.SetAttribute("Id", NamespaceConstants.wsu_Namespace, "_0");
            securityElement.AppendChild(timestampElement);

            var createdElement = xmlDocument.CreateElement(NamespaceConstants.wsu_Namespace, "Created", NamespaceConstants.wsu_Namespace);
            createdElement.InnerText = fechaCreacion;
            timestampElement.AppendChild(createdElement);

            var expires = xmlDocument.CreateElement(NamespaceConstants.wsu_Namespace, "Expires", NamespaceConstants.wsu_Namespace);
            expires.InnerText = fechaExpiracion;
            timestampElement.AppendChild(expires);

            var binaryId = $"uuid-{uuid}-1";
            var binarySecurityTokenElement = xmlDocument.CreateElement(NamespaceConstants.wsse_Prefix, "BinarySecurityToken", NamespaceConstants.wsse_Namespace);
            binarySecurityTokenElement.SetAttribute("Id", NamespaceConstants.wsu_Namespace, $"uuid-{uuid}-1");
            binarySecurityTokenElement.SetAttribute("ValueType", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-x509-token-profile-1.0#X509v3");
            binarySecurityTokenElement.SetAttribute("EncodingType", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-soap-message-security-1.0#Base64Binary");
            binarySecurityTokenElement.InnerText = Convert.ToBase64String(certificate.RawData);
            securityElement.AppendChild(binarySecurityTokenElement);

            var securityTokenReferenceElement = xmlDocument.CreateElement(NamespaceConstants.wsse_Prefix, "SecurityTokenReference", NamespaceConstants.wsse_Namespace);
            XmlElement keyIdentifier = xmlDocument.CreateElement(NamespaceConstants.wsse_Prefix, "Reference", NamespaceConstants.wsse_Namespace);
            XmlAttribute valueType = xmlDocument.CreateAttribute("ValueType");
            valueType.Value = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-x509-token-profile-1.0#X509v3";
            keyIdentifier.Attributes.Append(valueType);
            XmlAttribute encodingType = xmlDocument.CreateAttribute("URI");
            encodingType.Value = $"#{binaryId}";
            keyIdentifier.Attributes.Append(encodingType);
            securityTokenReferenceElement.AppendChild(keyIdentifier);

            var signatureElement = FirmarXmlWithReference(timestampElement, certificate, "#_0", securityTokenReferenceElement);
            securityElement.AppendChild(signatureElement);

            var bodyElement = xmlDocument.CreateElement(NamespaceConstants.S11_Prefix, "Body", NamespaceConstants.S11_Namespace);
            envelopElement.AppendChild(bodyElement);

            var autenticaElement = xmlDocument.CreateElement("Autentica");
            autenticaElement.SetAttribute("xmlns", "http://DescargaMasivaTerceros.gob.mx");
            bodyElement.AppendChild(autenticaElement);

            return xmlDocument.OuterXml;
        }
    }
}