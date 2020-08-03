﻿using System.Collections.Generic;

namespace Core.Domain.ValueObjects
{
    public class CertificadoSat : ValueObject
    {
        private CertificadoSat()
        {
        }

        public byte[] Certificado { get; private set; }
        public string Contrasena { get; private set; }
        public string Rfc { get; private set; }

        public static CertificadoSat CreateInstance(byte[] certificado, string contrasena, string rfc)
        {
            return new CertificadoSat
            {
                Certificado = certificado,
                Contrasena = contrasena,
                Rfc = rfc
            };
        }

        public static CertificadoSat CreateSinAsignar()
        {
            return CreateInstance(null, "", "");
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Certificado;
            yield return Contrasena;
            yield return Rfc;
        }
    }
}