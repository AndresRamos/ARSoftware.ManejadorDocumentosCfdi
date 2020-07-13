using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Data;
using Caliburn.Micro;
using Core.Application.Roles.Commands.EliminarRol;
using Core.Application.Roles.Models;
using Core.Application.Roles.Queries.BuscarRoles;
using MahApps.Metro.Controls.Dialogs;
using MediatR;

namespace Presentation.WpfApp.ViewModels.Roles
{
    public sealed class ListaRolesViewModel : Screen
    {
        private readonly IDialogCoordinator _dialogCoordinator;
        private readonly IMediator _mediator;
        private readonly IWindowManager _windowManager;
        private string _filtro;
        private RolDto _rolSeleccionado;

        public ListaRolesViewModel(IMediator mediator, IWindowManager windowManager, IDialogCoordinator dialogCoordinator)
        {
            _mediator = mediator;
            _windowManager = windowManager;
            _dialogCoordinator = dialogCoordinator;
            DisplayName = "Roles";
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

        public BindableCollection<RolDto> Roles { get; } = new BindableCollection<RolDto>();

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

        public bool CanEditarRolAsync => RolSeleccionado != null;

        public bool CanEliminarRolAsync => RolSeleccionado != null;

        public async Task InicializarAsync()
        {
            await CargarRolesAsync();
        }

        public async Task CargarRolesAsync()
        {
            Roles.Clear();
            Roles.AddRange(await _mediator.Send(new BuscarRolesQuery()));
        }

        public async Task CrearNuevoRolAsync()
        {
            try
            {
                var viewModel = IoC.Get<EditarRolViewModel>();
                await viewModel.InicializarAsync(null);
                await _windowManager.ShowDialogAsync(viewModel);
                await CargarRolesAsync();
            }
            catch (Exception e)
            {
                await _dialogCoordinator.ShowMessageAsync(this, "Error", e.ToString());
            }
            finally
            {
                RaiseGuards();
            }
        }

        public async Task EditarRolAsync()
        {
            try
            {
                var viewModel = IoC.Get<EditarRolViewModel>();
                await viewModel.InicializarAsync(RolSeleccionado.Id);
                await _windowManager.ShowDialogAsync(viewModel);
                await CargarRolesAsync();
            }
            catch (Exception e)
            {
                await _dialogCoordinator.ShowMessageAsync(this, "Error", e.ToString());
            }
            finally
            {
                RaiseGuards();
            }
        }

        public async Task EliminarRolAsync()
        {
            try
            {
                var messageDialogResult = await _dialogCoordinator.ShowMessageAsync(this,
                    "Eliminar Rol?",
                    $"Esta seguro de querer eliminar el rol {RolSeleccionado.Nombre}?",
                    MessageDialogStyle.AffirmativeAndNegative,
                    new MetroDialogSettings
                    {
                        AffirmativeButtonText = "Si",
                        NegativeButtonText = "No"
                    });
                if (messageDialogResult != MessageDialogResult.Affirmative)
                {
                    return;
                }

                await _mediator.Send(new EliminarRolCommand(RolSeleccionado.Id));
                await CargarRolesAsync();
            }
            catch (Exception e)
            {
                await _dialogCoordinator.ShowMessageAsync(this, "Error", e.ToString());
            }
            finally
            {
                RaiseGuards();
            }
        }

        private void RaiseGuards()
        {
            NotifyOfPropertyChange(() => CanEditarRolAsync);
            NotifyOfPropertyChange(() => CanEliminarRolAsync);
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
}