﻿using System;
using System.Security.Cryptography.X509Certificates;
using System.Xml;

namespace Infrastructure.Sat.Services
{
    public class GenerarSolicitudService : SoapRequestBase
    {
        public GenerarSolicitudService(string url, string action)
            : base(url, action)
        {
        }

        #region Crea el XML para enviar

        public string Generate(X509Certificate2 certificate, string rfcEmisor, string rfcReceptor, string rfcSolicitante, string fechaInicial = "", string fechaFinal = "", string tipoSolicitud = "CFDI")
        {
            FixFecha(fechaInicial, fechaFinal, out fechaInicial, out fechaFinal);
            var canonicalTimestamp = "<des:SolicitaDescarga xmlns:des=\"http://DescargaMasivaTerceros.sat.gob.mx\">"
                                     + "<des:solicitud RfcEmisor=\"" + rfcEmisor + "\" RfcReceptor=\"" + rfcReceptor + "\" RfcSolicitante=\"" + rfcSolicitante + "\" FechaInicial=\"" + fechaInicial + "\" FechaFinal=\"" + fechaFinal + "\" TipoSolicitud=\"CFDI\">"
                                     + "</des:solicitud>"
                                     + "</des:SolicitaDescarga>";

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
                               @"<des:SolicitaDescarga>" +
                               @"<des:solicitud RfcEmisor=""" + rfcEmisor +
                               @""" RfcReceptor =""" + rfcReceptor +
                               @""" RfcSolicitante=""" + rfcSolicitante +
                               @""" FechaInicial=""" + fechaInicial +
                               @""" FechaFinal =""" + fechaFinal +
                               @""" TipoSolicitud=""" + tipoSolicitud +
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
                               @"</des:SolicitaDescarga>" +
                               @"</s:Body>" +
                               @"</s:Envelope>";
            xml = soap_request;
            return soap_request;
        }

        #endregion

        private void FixFecha(string fechaInicial1, string fechaFinal1, out string fechaInicial, out string fechaFinal)
        {
            fechaInicial = fechaInicial1 + "T00:00:00";
            fechaFinal = fechaFinal1 + "T23:59:59";
        }

        public override string GetResult(XmlDocument xmlDoc)
        {
            var s = xmlDoc.GetElementsByTagName("SolicitaDescargaResult")[0].Attributes["IdSolicitud"].Value;
            return s;
        }
    }
}