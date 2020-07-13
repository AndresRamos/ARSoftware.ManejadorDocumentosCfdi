using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Data;
using Caliburn.Micro;
using Core.Application.Usuarios.Models;
using Core.Application.Usuarios.Queries.BuscarUsuarios;
using MahApps.Metro.Controls.Dialogs;
using MediatR;

namespace Presentation.WpfApp.ViewModels.Usuarios
{
    public sealed class ListaUsuariosViewModel : Screen
    {
        private readonly IDialogCoordinator _dialogCoordinator;
        private readonly IMediator _mediator;
        private readonly IWindowManager _windowManager;
        private string _filtro;
        private UsuarioDto _usuarioSeleccionado;

        public ListaUsuariosViewModel(IMediator mediator, IWindowManager windowManager, IDialogCoordinator dialogCoordinator)
        {
            _mediator = mediator;
            _windowManager = windowManager;
            _dialogCoordinator = dialogCoordinator;
            DisplayName = "Usuarios";
            UsuariosView = CollectionViewSource.GetDefaultView(Usuarios);
            UsuariosView.Filter = UsuariosView_Filter;
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
                UsuariosView.Refresh();
            }
        }

        public BindableCollection<UsuarioDto> Usuarios { get; } = new BindableCollection<UsuarioDto>();

        public ICollectionView UsuariosView { get; }

        public UsuarioDto UsuarioSeleccionado
        {
            get => _usuarioSeleccionado;
            set
            {
                if (Equals(value, _usuarioSeleccionado))
                {
                    return;
                }

                _usuarioSeleccionado = value;
                NotifyOfPropertyChange(() => UsuarioSeleccionado);
                RaiseGuards();
            }
        }

        public bool CanEditarUsuarioAsync => UsuarioSeleccionado != null;

        public bool CanEliminarUsuarioAsync => UsuarioSeleccionado != null;

        public async Task InicializarAsync()
        {
            await CargarUsuariosAsync();
        }

        private async Task CargarUsuariosAsync()
        {
            Usuarios.Clear();
            Usuarios.AddRange(await _mediator.Send(new BuscarUsuariosQuery()));
        }

        public async Task CrearUsuarioAsync()
        {
            try
            {
                var viewModel = IoC.Get<CrearUsuarioViewModel>();
                await _windowManager.ShowDialogAsync(viewModel);
                await CargarUsuariosAsync();
            }
            catch (Exception e)
            {
                await _dialogCoordinator.ShowMessageAsync(this, "Error", e.ToString());
            }
        }

        public async Task EditarUsuarioAsync()
        {
            try
            {
                var viewModel = IoC.Get<EditarUsuarioViewModel>();
                await viewModel.InicializarAsync(UsuarioSeleccionado.Id);
                await _windowManager.ShowDialogAsync(viewModel);
                await CargarUsuariosAsync();
            }
            catch (Exception e)
            {
                await _dialogCoordinator.ShowMessageAsync(this, "Error", e.ToString());
            }
        }

        public async Task EliminarUsuarioAsync()
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception e)
            {
                await _dialogCoordinator.ShowMessageAsync(this, "Error", e.ToString());
            }
        }

        private void RaiseGuards()
        {
            NotifyOfPropertyChange(() => CanEditarUsuarioAsync);
            NotifyOfPropertyChange(() => CanEliminarUsuarioAsync);
        }

        private bool UsuariosView_Filter(object obj)
        {
            if (!(obj is UsuarioDto usuario))
            {
                throw new ArgumentNullException(nameof(obj));
            }

            return string.IsNullOrEmpty(Filtro) ||
                   usuario.PrimerNombre.IndexOf(Filtro, StringComparison.OrdinalIgnoreCase) >= 0 ||
                   usuario.Apellido.IndexOf(Filtro, StringComparison.OrdinalIgnoreCase) >= 0 ||
                   usuario.Email.IndexOf(Filtro, StringComparison.OrdinalIgnoreCase) >= 0 ||
                   usuario.NombreUsuario.IndexOf(Filtro, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}