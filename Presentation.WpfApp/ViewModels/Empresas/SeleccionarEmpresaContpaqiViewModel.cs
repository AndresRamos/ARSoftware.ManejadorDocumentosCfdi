using System.ComponentModel;
using System.Windows.Data;
using Caliburn.Micro;
using Core.Application.Empresas.Models;

namespace Presentation.WpfApp.ViewModels.Empresas;

public sealed class SeleccionarEmpresaContpaqiViewModel : Screen
{
    private EmpresaContpaqiDto _empresaSeleccionada;
    private string _filtro;

    public SeleccionarEmpresaContpaqiViewModel()
    {
        DisplayName = "Seleccionar Empresa Contpaqi";
        EmpresasView = CollectionViewSource.GetDefaultView(Empresas);
        EmpresasView.Filter = EmpresasView_Filter;
    }

    public string Filtro
    {
        get => _filtro;
        set
        {
            if (value == _filtro)
                return;

            _filtro = value;
            NotifyOfPropertyChange(() => Filtro);
            EmpresasView.Refresh();
        }
    }

    public BindableCollection<EmpresaContpaqiDto> Empresas { get; } = new();

    public ICollectionView EmpresasView { get; }

    public EmpresaContpaqiDto EmpresaSeleccionada
    {
        get => _empresaSeleccionada;
        set
        {
            if (Equals(value, _empresaSeleccionada))
                return;

            _empresaSeleccionada = value;
            NotifyOfPropertyChange(() => EmpresaSeleccionada);
        }
    }

    public bool SeleccionoEmpresa { get; private set; }

    public void Inicializar(IEnumerable<EmpresaContpaqiDto> empresas)
    {
        SeleccionoEmpresa = false;
        Empresas.Clear();
        Empresas.AddRange(empresas);
    }

    public async Task Seleccionar()
    {
        SeleccionoEmpresa = true;
        await TryCloseAsync();
    }

    public async Task Cancelar()
    {
        EmpresaSeleccionada = null;
        SeleccionoEmpresa = false;
        await TryCloseAsync();
    }

    private bool EmpresasView_Filter(object obj)
    {
        if (!(obj is EmpresaContpaqiDto empresa))
            throw new ArgumentNullException(nameof(obj));

        return string.IsNullOrEmpty(Filtro) || empresa.Nombre.IndexOf(Filtro, StringComparison.OrdinalIgnoreCase) >= 0;
    }
}
