using System.ComponentModel;
using System.Windows.Data;
using Caliburn.Micro;
using Common.Models;

namespace Presentation.WpfApp.ViewModels.Permisos;

public sealed class SeleccionarPermisoAplicacionViewModel : Screen
{
    private string _filtro;
    private PermisoAplicacionDto _permisoAplicacionSeleccionado;

    public SeleccionarPermisoAplicacionViewModel()
    {
        DisplayName = "Permisos De Aplicacion";
        PermisosView = CollectionViewSource.GetDefaultView(Permisos);
        PermisosView.Filter = PermisosView_Filter;
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
            PermisosView.Refresh();
        }
    }

    public BindableCollection<PermisoAplicacionDto> Permisos { get; } = new();

    public ICollectionView PermisosView { get; }

    public PermisoAplicacionDto PermisoAplicacionSeleccionado
    {
        get => _permisoAplicacionSeleccionado;
        set
        {
            if (Equals(value, _permisoAplicacionSeleccionado))
                return;

            _permisoAplicacionSeleccionado = value;
            NotifyOfPropertyChange(() => PermisoAplicacionSeleccionado);
            RaiseGuards();
        }
    }

    public bool SeleccionoPermiso { get; private set; }

    public bool CanSeleccionar => PermisoAplicacionSeleccionado != null;

    public void Inicializar(IEnumerable<PermisoAplicacionDto> permisos)
    {
        Permisos.Clear();
        Permisos.AddRange(permisos);
    }

    public async Task Seleccionar()
    {
        SeleccionoPermiso = true;
        await TryCloseAsync();
    }

    public async Task Cancelar()
    {
        SeleccionoPermiso = false;
        PermisoAplicacionSeleccionado = null;
        await TryCloseAsync();
    }

    private void RaiseGuards()
    {
        NotifyOfPropertyChange(() => CanSeleccionar);
    }

    private bool PermisosView_Filter(object obj)
    {
        if (!(obj is PermisoAplicacionDto permiso))
            throw new ArgumentNullException(nameof(obj));

        return string.IsNullOrEmpty(Filtro) ||
               permiso.Nombre?.IndexOf(Filtro, StringComparison.OrdinalIgnoreCase) >= 0 ||
               permiso.Descripcion?.IndexOf(Filtro, StringComparison.OrdinalIgnoreCase) >= 0 ||
               permiso.Grupo?.IndexOf(Filtro, StringComparison.OrdinalIgnoreCase) >= 0;
    }
}
