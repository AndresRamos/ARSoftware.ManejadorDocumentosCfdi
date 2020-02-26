using Core.Domain.ValueObjects;

namespace Core.Domain.Entities
{
    public class ConfiguracionGeneral
    {
        public ConfiguracionGeneral()
        {
            CertificadoSat = new CertificadoSat(null, "", "");
        }

        public int Id { get; set; }
        public CertificadoSat CertificadoSat { get; set; }
        public string RutaDirectorioDescargas { get; set; }
    }
}