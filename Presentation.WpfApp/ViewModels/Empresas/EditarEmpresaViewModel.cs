using System;
using System.Threading.Tasks;
using Caliburn.Micro;
using Core.Application.Empresas.Commands.ActualizarEmpresaPerfil;
using Core.Application.Empresas.Commands.CrearEmpresa;
using Core.Application.Empresas.Models;
using Core.Application.Empresas.Queries.BuscarEmpresaPorId;
using MahApps.Metro.Controls.Dialogs;
using MediatR;

namespace Presentation.WpfApp.ViewModels.Empresas;

public sealed class EditarEmpresaViewModel : Screen
{
    private readonly IDialogCoordinator _dialogCoordinator;
    private readonly IMediator _mediator;
    private string _nombre;

    public EditarEmpresaViewModel(IMediator mediator, IDialogCoordinator dialogCoordinator)
    {
        _mediator = mediator;
        _dialogCoordinator = dialogCoordinator;
        DisplayName = "Editar Empresa";
    }

    public int? EmpresaId { get; private set; }

    public string Nombre
    {
        get => _nombre;
        set
        {
            if (_nombre == value)
            {
                return;
            }

            _nombre = value;
            NotifyOfPropertyChange(() => Nombre);
        }
    }

    public async Task InicializarAsync(int? id)
    {
        if (id == null)
        {
        }
        else
        {
            EmpresaPerfilDto empresa = await _mediator.Send(new BuscarEmpresaPorIdQuery(id.Value));
            EmpresaId = empresa.Id;
            Nombre = empresa.Nombre;
        }
    }

    public async Task GuardarAsync()
    {
        try
        {
            if (EmpresaId == null)
            {
                EmpresaId = await _mediator.Send(new CrearEmpresaCommand(Nombre));
            }
            else
            {
                await _mediator.Send(new ActualizarEmpresaPerfilCommand(EmpresaId.Value, Nombre));
            }

            await _dialogCoordinator.ShowMessageAsync(this, "Empresa Guardada", "La empresa se guardo exitosamente.");
        }
        catch (Exception e)
        {
            await _dialogCoordinator.ShowMessageAsync(this, "Error", e.ToString());
        }
    }

    public async Task CancelarAsync()
    {
        await TryCloseAsync();
    }
}
