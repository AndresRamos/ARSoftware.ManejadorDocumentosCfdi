using System.ComponentModel;
using System.Runtime.CompilerServices;
using Core.Application.Annotations;

namespace Core.Application.ConfiguracionGeneral.Models;

public class ConfiguracionGeneralDto : INotifyPropertyChanged
{
    private CertificadoSatDto _certificadoSat;
    private ConfiguracionContpaqiComercialDto _configuracionContpaqiComercial;
    private ConfiguracionContpaqiContabilidadDto _configuracionContpaqiContabilidad;
    private int _id;
    private string _rutaDirectorioDescargas;

    public ConfiguracionGeneralDto(int id,
                                   CertificadoSatDto certificadoSat,
                                   string rutaDirectorioDescargas,
                                   ConfiguracionContpaqiComercialDto configuracionContpaqiComercial,
                                   ConfiguracionContpaqiContabilidadDto configuracionContpaqiContabilidad)
    {
        Id = id;
        CertificadoSat = certificadoSat;
        RutaDirectorioDescargas = rutaDirectorioDescargas;
        ConfiguracionContpaqiComercial = configuracionContpaqiComercial;
        ConfiguracionContpaqiContabilidad = configuracionContpaqiContabilidad;
    }

    public int Id
    {
        get => _id;
        set
        {
            if (value == _id)
            {
                return;
            }

            _id = value;
            OnPropertyChanged();
        }
    }

    public CertificadoSatDto CertificadoSat
    {
        get => _certificadoSat;
        set
        {
            if (Equals(value, _certificadoSat))
            {
                return;
            }

            _certificadoSat = value;
            OnPropertyChanged();
        }
    }

    public string RutaDirectorioDescargas
    {
        get => _rutaDirectorioDescargas;
        set
        {
            if (value == _rutaDirectorioDescargas)
            {
                return;
            }

            _rutaDirectorioDescargas = value;
            OnPropertyChanged();
        }
    }

    public ConfiguracionContpaqiComercialDto ConfiguracionContpaqiComercial
    {
        get => _configuracionContpaqiComercial;
        set
        {
            if (Equals(value, _configuracionContpaqiComercial))
            {
                return;
            }

            _configuracionContpaqiComercial = value;
            OnPropertyChanged();
        }
    }

    public ConfiguracionContpaqiContabilidadDto ConfiguracionContpaqiContabilidad
    {
        get => _configuracionContpaqiContabilidad;
        set
        {
            if (Equals(value, _configuracionContpaqiContabilidad))
            {
                return;
            }

            _configuracionContpaqiContabilidad = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
