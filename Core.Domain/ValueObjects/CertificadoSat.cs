using System.Collections.Generic;

namespace Core.Domain.ValueObjects
{
    public class CertificadoSat : ValueObject
    {
        private CertificadoSat()
        {
        }

        public CertificadoSat(byte[] certificado, string contrasena, string rfc)
        {
            Certificado = certificado;
            Contrasena = contrasena;
            Rfc = rfc;
        }

        public byte[] Certificado { get; private set; }
        public string Contrasena { get; private set; }
        public string Rfc { get; private set; }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Certificado;
            yield return Contrasena;
            yield return Rfc;
        }
    }
}