using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Data;
using Caliburn.Micro;
using Core.Application.Empresas.Commands.EliminarEmpresa;
using Core.Application.Empresas.Models;
using Core.Application.Empresas.Queries.BuscarEmpresas;
using MahApps.Metro.Controls.Dialogs;
using MediatR;

namespace Presentation.WpfApp.ViewModels.Empresas;

public sealed class ListaEmpresasViewModel : Screen
{
    private readonly IDialogCoordinator _dialogCoordinator;
    private readonly IMediator _mediator;
    private readonly IWindowManager _windowManager;
    private EmpresaPerfilDto _empresaSeleccionada;
    private string _filtro;

    public ListaEmpresasViewModel(IMediator mediator, IDialogCoordinator dialogCoordinator, IWindowManager windowManager)
    {
        _mediator = mediator;
        _dialogCoordinator = dialogCoordinator;
        _windowManager = windowManager;
        DisplayName = "Lista De Empresas";
        EmpresasView = CollectionViewSource.GetDefaultView(Empresas);
        EmpresasView.Filter = EmpresasView_Filter;
    }

    public string Filtro
    {
        get => _filtro;
        set
        {
            if (_filtro == value)
            {
                return;
            }

            _filtro = value;
            NotifyOfPropertyChange(() => Filtro);
            EmpresasView.Refresh();
        }
    }

    public BindableCollection<EmpresaPerfilDto> Empresas { get; } = new();

    public ICollectionView EmpresasView { get; }

    public EmpresaPerfilDto EmpresaSeleccionada
    {
        get => _empresaSeleccionada;
        set
        {
            if (_empresaSeleccionada == value)
            {
                return;
            }

            _empresaSeleccionada = value;
            NotifyOfPropertyChange(() => EmpresaSeleccionada);
            RaiseGuards();
        }
    }

    public bool CanEditarEmpresaAsync => EmpresaSeleccionada != null;
    public bool CanEliminarEmpresaAsync => EmpresaSeleccionada != null;

    public async Task InicializarAsync()
    {
        await CargarEmpresasAsync();
    }

    private async Task CargarEmpresasAsync()
    {
        Empresas.Clear();
        Empresas.AddRange(await _mediator.Send(new BuscarEmpresasQuery()));
    }

    public async Task CrearEmpresaAsync()
    {
        try
        {
            var editarEmpresaViewModel = IoC.Get<EditarEmpresaViewModel>();
            await editarEmpresaViewModel.InicializarAsync(null);
            await _windowManager.ShowDialogAsync(editarEmpresaViewModel);
            await CargarEmpresasAsync();
        }
        catch (Exception e)
        {
            await _dialogCoordinator.ShowMessageAsync(this, "Error", e.ToString());
        }
    }

    public async Task EditarEmpresaAsync()
    {
        try
        {
            var editarEmpresaViewModel = IoC.Get<EditarEmpresaViewModel>();
            await editarEmpresaViewModel.InicializarAsync(EmpresaSeleccionada.Id);
            await _windowManager.ShowDialogAsync(editarEmpresaViewModel);
            await CargarEmpresasAsync();
        }
        catch (Exception e)
        {
            await _dialogCoordinator.ShowMessageAsync(this, "Error", e.ToString());
        }
    }

    public async Task EliminarEmpresaAsync()
    {
        try
        {
            MessageDialogResult messageDialogResult = await _dialogCoordinator.ShowMessageAsync(this,
                "Eliminar Empresa",
                "Esta seguro de querer eliminar la empresa?",
                MessageDialogStyle.AffirmativeAndNegative,
                new MetroDialogSettings { AffirmativeButtonText = "Si", NegativeButtonText = "No" });
            if (messageDialogResult == MessageDialogResult.Affirmative)
            {
                await _mediator.Send(new EliminarEmpresaCommand(EmpresaSeleccionada.Id));
                await CargarEmpresasAsync();
            }
        }
        catch (Exception e)
        {
            await _dialogCoordinator.ShowMessageAsync(this, "Error", e.ToString());
        }
    }

    private void RaiseGuards()
    {
        NotifyOfPropertyChange(() => CanEditarEmpresaAsync);
        NotifyOfPropertyChange(() => CanEliminarEmpresaAsync);
    }

    private bool EmpresasView_Filter(object obj)
    {
        if (obj is null)
        {
            throw new ArgumentNullException(nameof(obj));
        }

        if (!(obj is EmpresaPerfilDto empresa))
        {
            throw new InvalidOperationException($"El objecto a filtrar no es de tipo {typeof(EmpresaPerfilDto)}.");
        }

        return string.IsNullOrWhiteSpace(Filtro) || empresa.Nombre.IndexOf(Filtro, StringComparison.OrdinalIgnoreCase) >= 0;
    }
}
