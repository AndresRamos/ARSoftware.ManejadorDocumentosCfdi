using System;
using System.Threading.Tasks;
using Caliburn.Micro;
using Core.Application.Usuarios.Commands.CrearUsuario;
using MahApps.Metro.Controls.Dialogs;
using MediatR;

namespace Presentation.WpfApp.ViewModels.Usuarios
{
    public sealed class CrearUsuarioViewModel : Screen
    {
        private readonly IDialogCoordinator _dialogCoordinator;
        private readonly IMediator _mediator;
        private string _apellido;
        private string _contrasena;
        private string _email;
        private string _nombreUsuario;
        private string _primerNombre;

        public CrearUsuarioViewModel(IMediator mediator, IDialogCoordinator dialogCoordinator)
        {
            _mediator = mediator;
            _dialogCoordinator = dialogCoordinator;
            DisplayName = "Crear Usuario";
        }

        public string PrimerNombre
        {
            get => _primerNombre;
            set
            {
                if (value == _primerNombre)
                {
                    return;
                }

                _primerNombre = value;
                NotifyOfPropertyChange(() => PrimerNombre);
            }
        }

        public string Apellido
        {
            get => _apellido;
            set
            {
                if (value == _apellido)
                {
                    return;
                }

                _apellido = value;
                NotifyOfPropertyChange(() => Apellido);
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                if (value == _email)
                {
                    return;
                }

                _email = value;
                NotifyOfPropertyChange(() => Email);
            }
        }

        public string NombreUsuario
        {
            get => _nombreUsuario;
            set
            {
                if (value == _nombreUsuario)
                {
                    return;
                }

                _nombreUsuario = value;
                NotifyOfPropertyChange(() => NombreUsuario);
            }
        }

        public string Contrasena
        {
            get => _contrasena;
            set
            {
                if (value == _contrasena)
                {
                    return;
                }

                _contrasena = value;
                NotifyOfPropertyChange(() => Contrasena);
            }
        }

        public async Task CrearUsuarioAsync()
        {
            try
            {
                await _mediator.Send(new CrearUsuarioCommand(PrimerNombre, Apellido, Email, NombreUsuario, Contrasena));
                await TryCloseAsync();
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
    }
}
