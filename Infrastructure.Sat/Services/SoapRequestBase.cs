using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml;
using Infrastructure.Sat.Models;
using NLog;

namespace Infrastructure.Sat.Services
{
    public abstract class SoapRequestBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly string _soapAction;
        private readonly string _url;
        protected HttpStatusCode _httpStatusCode;

        protected SoapRequestBase(string url, string soapAction)
        {
            _url = url;
            _soapAction = soapAction;
        }

        public SolicitudResult Send(string xml, string autorization)
        {
            if (xml == null)
            {
                throw new ArgumentNullException(nameof(xml), "El xml no puede ser nulo.");
            }

            try
            {
                var httpWebRequest = CrearHttpWebRequest();

                if (!string.IsNullOrEmpty(autorization))
                {
                    httpWebRequest.Headers.Add(HttpRequestHeader.Authorization, autorization);
                }

                using (var stream = httpWebRequest.GetRequestStream())
                using (var streamWriter = new StreamWriter(stream))
                {
                    streamWriter.Write(xml);
                }

                using (var httpWebResponse = (HttpWebResponse) httpWebRequest.GetResponse())
                {
                    using (var streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                    {
                        _httpStatusCode = httpWebResponse.StatusCode;
                        var requestResponse = streamReader.ReadToEnd();
                        return GetResult(requestResponse);
                    }
                }
            }
            catch (WebException e)
            {
                Logger.Error(e);
                throw;
            }
            catch (Exception e)
            {
                Logger.Error(e);
                throw;
            }
        }

        public abstract SolicitudResult GetResult(string webResponse);

        private HttpWebRequest CrearHttpWebRequest()
        {
            var webRequest = (HttpWebRequest) WebRequest.Create(_url);
            webRequest.Timeout = 180000;
            webRequest.Method = "POST";
            webRequest.ContentType = "text/xml; charset=utf-8";
            webRequest.Headers.Add("SOAPAction: " + _soapAction);
            return webRequest;
        }

        public string CreateDigest(string sourceData)
        {
            var data = GetBytes(sourceData);
            return Convert.ToBase64String(HashAlgorithm.Create("SHA1").ComputeHash(data));
        }

        public string Sign(string sourceData, X509Certificate2 certificate)
        {
            var data = GetBytes(sourceData);
            byte[] signature;

            using (var rsaCryptoServiceProvider = certificate.GetRSAPrivateKey())
            {
                signature = rsaCryptoServiceProvider.SignData(data, HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1);
            }

            return Convert.ToBase64String(signature);
        }

        private byte[] GetBytes(string sourceData)
        {
            return Encoding.Default.GetBytes(sourceData);
        }

        public static XmlElement FirmarXml(XmlElement xmlElement, X509Certificate2 varCertificado)
        {
            var varXMLFirmado = new SignedXml(xmlElement);
            varXMLFirmado.SigningKey = varCertificado.GetRSAPrivateKey();
            varXMLFirmado.SignedInfo.SignatureMethod = SignedXml.XmlDsigRSASHA1Url;

            var varReferencia = new Reference();
            varReferencia.Uri = "";
            varReferencia.DigestMethod = SignedXml.XmlDsigSHA1Url;
            varReferencia.AddTransform(new XmlDsigEnvelopedSignatureTransform());
            varXMLFirmado.AddReference(varReferencia);

            var keyData = new KeyInfoX509Data(varCertificado);
            keyData.AddIssuerSerial(varCertificado.Issuer, varCertificado.SerialNumber);

            var varKeyInfo = new KeyInfo();
            varKeyInfo.AddClause(keyData);
            //varKeyInfo.AddClause(new RSAKeyValue((RSA) varCertificado.PublicKey.Key));
            varXMLFirmado.KeyInfo = varKeyInfo;
            varXMLFirmado.ComputeSignature();

            return varXMLFirmado.GetXml();
        }

    }
}