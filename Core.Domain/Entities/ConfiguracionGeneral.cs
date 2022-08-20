using Core.Domain.ValueObjects;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Core.Domain.Entities
{
    public class ConfiguracionGeneral
    {
        public ConfiguracionGeneral()
        {
            CertificadoSat = CertificadoSat.CreateSinAsignar();
            ConfiguracionContpaqiComercial = ConfiguracionContpaqiComercial.CreateSinAsignar();
            ConfiguracionContpaqiContabilidad = ConfiguracionContpaqiContabilidad.CreateSinAsignar();
        }

        public int Id { get; set; }
        public CertificadoSat CertificadoSat { get; set; }
        public string RutaDirectorioDescargas { get; set; }
        public ConfiguracionContpaqiComercial ConfiguracionContpaqiComercial { get; set; }
        public ConfiguracionContpaqiContabilidad ConfiguracionContpaqiContabilidad { get; set; }
    }
}
