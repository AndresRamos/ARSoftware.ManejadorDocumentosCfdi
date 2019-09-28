using System.Security.Cryptography.X509Certificates;

namespace Infrastructure.Sat.Services
{
    public static class CertificadoService
    {
        public static X509Certificate2 ObtenerCertificado(byte[] certificado, string password)
        {
           return new X509Certificate2(certificado, password, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);
        }
    }
}