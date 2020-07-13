using Core.Domain.ValueObjects;

namespace Core.Domain.Entities
{
    public class ConfiguracionGeneral
    {
        public ConfiguracionGeneral()
        {
            CertificadoSat = new CertificadoSat(null, "", "");
            ConfiguracionContpaqiComercial = ConfiguracionContpaqiComercial.CreateInstance("", EmpresaContpaqi.CreateInstance("", "", ""));
            ConfiguracionContpaqiContabilidad = ConfiguracionContpaqiContabilidad.CreateInstance("", EmpresaContpaqi.CreateInstance("", "", ""));
        }

        public int Id { get; set; }
        public CertificadoSat CertificadoSat { get; set; }
        public string RutaDirectorioDescargas { get; set; }
        public ConfiguracionContpaqiComercial ConfiguracionContpaqiComercial { get; set; }
        public ConfiguracionContpaqiContabilidad ConfiguracionContpaqiContabilidad { get; set; }
    }
}