using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
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

        public static XmlElement FirmarXmlWithReference(XmlElement xmlElement, X509Certificate2 varCertificado, string reference,  XmlElement refer)
        {
            var varXMLFirmado = new SignedXmlWithId(xmlElement);
            varXMLFirmado.SigningKey = varCertificado.GetRSAPrivateKey();
            varXMLFirmado.SignedInfo.SignatureMethod = SignedXml.XmlDsigRSASHA1Url;
            varXMLFirmado.SignedInfo.CanonicalizationMethod = SignedXml.XmlDsigExcC14NTransformUrl;
            var varReferencia = new Reference();
            varReferencia.Uri = reference;
            varReferencia.DigestMethod = SignedXml.XmlDsigSHA1Url;
            varReferencia.AddTransform(new XmlDsigExcC14NTransform());
            varXMLFirmado.AddReference(varReferencia);

            //var keyData = new KeyInfoX509Data(varCertificado);
            //keyData.AddIssuerSerial(varCertificado.Issuer, varCertificado.SerialNumber);

            var varKeyInfo = new KeyInfo();
            //varKeyInfo.AddClause(keyData);
            //varKeyInfo.AddClause(new RSAKeyValue((RSA) varCertificado.PublicKey.Key));
            var infoNode = new KeyInfoNode();
            infoNode.Value = refer;
            varKeyInfo.AddClause(infoNode);

            varXMLFirmado.KeyInfo = varKeyInfo;
            varXMLFirmado.ComputeSignature();

            return varXMLFirmado.GetXml();
        }

        public async Task<SolicitudResult> SendHttpClient(string xml, string autorization)
        {
            if (xml == null)
            {
                throw new ArgumentNullException(nameof(xml), "El xml no puede ser nulo.");
            }

            try
            {
                var httpClient = new HttpClient();
                var request = new HttpRequestMessage()
                {
                    RequestUri = new Uri(_url),
                    Method = HttpMethod.Post,
                };

                request.Headers.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Text.Xml));
                request.Headers.Add("SOAPAction", _soapAction);

                request.Content = new StringContent(xml, Encoding.UTF8, MediaTypeNames.Text.Xml);
                request.Content.Headers.ContentType = new MediaTypeHeaderValue(MediaTypeNames.Text.Xml);

                var response = await httpClient.SendAsync(request);

                var readAsStringAsync = await response.Content.ReadAsStringAsync();

                return GetResult(readAsStringAsync);
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
        public class SignedXmlWithId : SignedXml
        {
            public SignedXmlWithId(XmlDocument xml) : base(xml)
            {
            }

            public SignedXmlWithId(XmlElement xmlElement)
                : base(xmlElement)
            {
            }

            public override XmlElement GetIdElement(XmlDocument doc, string id)
            {
                // check to see if it's a standard ID reference
                XmlElement idElem = base.GetIdElement(doc, id);

                if (idElem == null)
                {
                    XmlNamespaceManager nsManager = new XmlNamespaceManager(doc.NameTable);
                    nsManager.AddNamespace("u", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd");

                    idElem = doc.SelectSingleNode("//*[@u:Id=\"" + id + "\"]", nsManager) as XmlElement;
                }

                return idElem;
            }
        }
    }
}