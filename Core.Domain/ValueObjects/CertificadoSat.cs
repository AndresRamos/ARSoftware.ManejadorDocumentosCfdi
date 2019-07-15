using System.Collections.Generic;

namespace Core.Domain.ValueObjects
{
    public class CertificadoSat : ValueObject
    {
        private CertificadoSat()
        {
        }

        public CertificadoSat(byte[] certificado, string contrasena, string rfcEmisor)
        {
            Certificado = certificado;
            Contrasena = contrasena;
            RfcEmisor = rfcEmisor;
        }

        public byte[] Certificado { get; private set; }
        public string Contrasena { get; private set; }
        public string RfcEmisor { get; private set; }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Certificado;
            yield return Contrasena;
            yield return RfcEmisor;
        }
    }
}