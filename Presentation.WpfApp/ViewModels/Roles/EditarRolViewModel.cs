using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using Common.Models;
using Core.Application.Permisos.Queries.BuscarPermisosAplicacion;
using Core.Application.Roles.Commands.ActualizarRol;
using Core.Application.Roles.Commands.CrearRol;
using Core.Application.Roles.Models;
using Core.Application.Roles.Queries.BuscarRolPorId;
using MahApps.Metro.Controls.Dialogs;
using MediatR;
using Presentation.WpfApp.ViewModels.Permisos;

namespace Presentation.WpfApp.ViewModels.Roles;

public sealed class EditarRolViewModel : Screen
{
    private readonly IDialogCoordinator _dialogCoordinator;
    private readonly IMediator _mediator;
    private readonly IWindowManager _windowManager;
    private string _descripcion;
    private int _id;
    private string _nombre;
    private PermisoAplicacionDto _permisoAplicacionSeleccionado;

    public EditarRolViewModel(IMediator mediator, IWindowManager windowManager, IDialogCoordinator dialogCoordinator)
    {
        _mediator = mediator;
        _windowManager = windowManager;
        _dialogCoordinator = dialogCoordinator;
        DisplayName = "Editar Rol";
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
            NotifyOfPropertyChange(() => Id);
        }
    }

    public string Nombre
    {
        get => _nombre;
        set
        {
            if (value == _nombre)
            {
                return;
            }

            _nombre = value;
            NotifyOfPropertyChange(() => Nombre);
        }
    }

    public string Descripcion
    {
        get => _descripcion;
        set
        {
            if (value == _descripcion)
            {
                return;
            }

            _descripcion = value;
            NotifyOfPropertyChange(() => Descripcion);
        }
    }

    public BindableCollection<PermisoAplicacionDto> Permisos { get; } = new();

    public PermisoAplicacionDto PermisoAplicacionSeleccionado
    {
        get => _permisoAplicacionSeleccionado;
        set
        {
            if (Equals(value, _permisoAplicacionSeleccionado))
            {
                return;
            }

            _permisoAplicacionSeleccionado = value;
            NotifyOfPropertyChange(() => PermisoAplicacionSeleccionado);
            RaiseGuards();
        }
    }

    public bool CanRemoverPermisoAsync => PermisoAplicacionSeleccionado != null;

    public async Task InicializarAsync(int? rolId)
    {
        if (rolId == null)
        {
            Permisos.Clear();
        }
        else
        {
            RolDto rol = await _mediator.Send(new BuscarRolPorIdQuery(rolId.Value));
            Id = rol.Id;
            Nombre = rol.Nombre;
            Descripcion = rol.Descripcion;
            Permisos.Clear();
            Permisos.AddRange(rol.Permisos);
        }
    }

    public async Task GuardarAsync()
    {
        try
        {
            if (Id == 0)
            {
                await _mediator.Send(new CrearRolCommand(Nombre, Descripcion, Permisos));
            }
            else
            {
                await _mediator.Send(new ActualizarRolCommand(Id, Nombre, Descripcion, Permisos));
            }

            await _dialogCoordinator.ShowMessageAsync(this, "Rol Guardado", "Rol guardado exitosamente.");
        }
        catch (Exception e)
        {
            await _dialogCoordinator.ShowMessageAsync(this, "Error", e.ToString());
        }
    }

    public async Task Cancelar()
    {
        await TryCloseAsync();
    }

    public async Task AgregarPermisoAsync()
    {
        try
        {
            IEnumerable<PermisoAplicacionDto> permisosAplicacion = await _mediator.Send(new BuscarPermisosAplicacionQuery());
            List<PermisoAplicacionDto> permisosUnicos = permisosAplicacion.Where(pa => Permisos.All(p => p.Nombre != pa.Nombre)).ToList();
            var viewModel = IoC.Get<SeleccionarPermisoAplicacionViewModel>();
            viewModel.Inicializar(permisosUnicos);
            await _windowManager.ShowDialogAsync(viewModel);
            if (viewModel.SeleccionoPermiso)
            {
                Permisos.Add(viewModel.PermisoAplicacionSeleccionado);
            }
        }
        catch (Exception e)
        {
            await _dialogCoordinator.ShowMessageAsync(this, "Error", e.ToString());
        }
    }

    public async Task RemoverPermisoAsync()
    {
        try
        {
            Permisos.Remove(PermisoAplicacionSeleccionado);
        }
        catch (Exception e)
        {
            await _dialogCoordinator.ShowMessageAsync(this, "Error", e.ToString());
        }
    }

    private void RaiseGuards()
    {
        NotifyOfPropertyChange(() => CanRemoverPermisoAsync);
    }
}
