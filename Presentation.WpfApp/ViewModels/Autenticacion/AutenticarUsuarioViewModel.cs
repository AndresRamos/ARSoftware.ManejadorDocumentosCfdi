using Caliburn.Micro;
using Core.Application.Autenticacion.Queries.ValidarCredencialesUsuario;
using Core.Application.Usuarios.Commands.CrearUsuarioAdministrador;
using Core.Application.Usuarios.Models;
using MahApps.Metro.Controls.Dialogs;
using MediatR;

namespace Presentation.WpfApp.ViewModels.Autenticacion;

public sealed class AutenticarUsuarioViewModel : Screen
{
    private readonly IDialogCoordinator _dialogCoordinator;
    private readonly IMediator _mediator;
    private string _contrasena;
    private string _nombreUsuario;

    public AutenticarUsuarioViewModel(IMediator mediator, IDialogCoordinator dialogCoordinator)
    {
        _mediator = mediator;
        _dialogCoordinator = dialogCoordinator;
        DisplayName = "Autenticar Usuario";
    }

    public string NombreUsuario
    {
        get => _nombreUsuario;
        set
        {
            if (value == _nombreUsuario)
                return;

            _nombreUsuario = value;
            NotifyOfPropertyChange(() => NombreUsuario);
            RaiseGuards();
        }
    }

    public string Contrasena
    {
        get => _contrasena;
        set
        {
            if (value == _contrasena)
                return;

            _contrasena = value;
            NotifyOfPropertyChange(() => Contrasena);
            RaiseGuards();
        }
    }

    public bool IsUsuarioAutenticado { get; private set; }

    public UsuarioDto Usuario { get; private set; }

    public bool CanAutenticarAsync => !string.IsNullOrWhiteSpace(NombreUsuario) && !string.IsNullOrWhiteSpace(Contrasena);

    public void Inicializar()
    {
        IsUsuarioAutenticado = false;
        Usuario = null;
        NombreUsuario = string.Empty;
        Contrasena = string.Empty;
    }

    public async Task AutenticarAsync()
    {
        try
        {
            await _mediator.Send(new CrearUsuarioAdministradorCommand());
            UsuarioDto usuario = await _mediator.Send(new ValidarCredencialesUsuarioQuery(NombreUsuario, Contrasena));
            if (usuario != null)
            {
                IsUsuarioAutenticado = true;
                Usuario = usuario;
                await TryCloseAsync();
            }
        }
        catch (Exception e)
        {
            await _dialogCoordinator.ShowMessageAsync(this, "Error", e.ToString());
        }
        finally
        {
            NombreUsuario = string.Empty;
            Contrasena = string.Empty;
            RaiseGuards();
        }
    }

    public async Task Cancelar()
    {
        IsUsuarioAutenticado = false;
        Usuario = null;
        await TryCloseAsync();
    }

    private void RaiseGuards()
    {
        NotifyOfPropertyChange(() => CanAutenticarAsync);
    }
}
