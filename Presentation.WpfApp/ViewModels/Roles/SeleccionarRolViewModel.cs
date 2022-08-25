using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Data;
using Caliburn.Micro;
using Core.Application.Roles.Models;

namespace Presentation.WpfApp.ViewModels.Roles;

public sealed class SeleccionarRolViewModel : Screen
{
    private string _filtro;
    private RolDto _rolSeleccionado;

    public SeleccionarRolViewModel()
    {
        DisplayName = "Seleccionar Rol";
        RolesView = CollectionViewSource.GetDefaultView(Roles);
        RolesView.Filter = RolesView_Filter;
    }

    public string Filtro
    {
        get => _filtro;
        set
        {
            if (value == _filtro)
            {
                return;
            }

            _filtro = value;
            NotifyOfPropertyChange(() => Filtro);
            RolesView.Refresh();
        }
    }

    public BindableCollection<RolDto> Roles { get; } = new();

    public ICollectionView RolesView { get; }

    public RolDto RolSeleccionado
    {
        get => _rolSeleccionado;
        set
        {
            if (Equals(value, _rolSeleccionado))
            {
                return;
            }

            _rolSeleccionado = value;
            NotifyOfPropertyChange(() => RolSeleccionado);
            RaiseGuards();
        }
    }

    public bool SeleccionoRol { get; private set; }

    public bool CanSeleccionar => RolSeleccionado != null;

    public void Inicializar(IEnumerable<RolDto> roles)
    {
        Roles.Clear();
        Roles.AddRange(roles);
    }

    public async Task Seleccionar()
    {
        SeleccionoRol = true;
        await TryCloseAsync();
    }

    public async Task Cancelar()
    {
        SeleccionoRol = false;
        RolSeleccionado = null;
        await TryCloseAsync();
    }

    private void RaiseGuards()
    {
        NotifyOfPropertyChange(() => CanSeleccionar);
    }

    private bool RolesView_Filter(object obj)
    {
        if (!(obj is RolDto rol))
        {
            throw new ArgumentNullException(nameof(obj));
        }

        return string.IsNullOrEmpty(Filtro) ||
               rol.Nombre.IndexOf(Filtro, StringComparison.OrdinalIgnoreCase) >= 0 ||
               rol.Descripcion.IndexOf(Filtro, StringComparison.OrdinalIgnoreCase) >= 0;
    }
}
