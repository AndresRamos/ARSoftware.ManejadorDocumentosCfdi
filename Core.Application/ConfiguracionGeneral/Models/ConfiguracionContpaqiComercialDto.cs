using System.ComponentModel;
using System.Runtime.CompilerServices;
using Core.Application.Annotations;
using Core.Application.Empresas.Models;

namespace Core.Application.ConfiguracionGeneral.Models;

public class ConfiguracionContpaqiComercialDto : INotifyPropertyChanged
{
    private string _contpaqiSqlConnectionString;
    private EmpresaContpaqiDto _empresa;

    public ConfiguracionContpaqiComercialDto(string contpaqiSqlConnectionString, EmpresaContpaqiDto empresa)
    {
        ContpaqiSqlConnectionString = contpaqiSqlConnectionString;
        Empresa = empresa;
    }

    public string ContpaqiSqlConnectionString
    {
        get => _contpaqiSqlConnectionString;
        set
        {
            if (value == _contpaqiSqlConnectionString)
            {
                return;
            }

            _contpaqiSqlConnectionString = value;
            OnPropertyChanged();
        }
    }

    public EmpresaContpaqiDto Empresa
    {
        get => _empresa;
        set
        {
            if (Equals(value, _empresa))
            {
                return;
            }

            _empresa = value;
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
