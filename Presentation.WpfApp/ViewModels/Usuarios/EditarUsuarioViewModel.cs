using System;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using Core.Application.Empresas.Models;
using Core.Application.Empresas.Queries.BuscarEmpresas;
using Core.Application.Empresas.Queries.BuscarEmpresasPermitidasPorUsuario;
using Core.Application.Roles.Models;
using Core.Application.Roles.Queries.BuscarRoles;
using Core.Application.Usuarios.Commands.ActualizarPerfilUsuario;
using Core.Application.Usuarios.Commands.AgregarEmpresaPermitida;
using Core.Application.Usuarios.Commands.AgregarRol;
using Core.Application.Usuarios.Commands.CambiarContrasena;
using Core.Application.Usuarios.Commands.RemoverEmpresaPermitida;
using Core.Application.Usuarios.Commands.RemoverRol;
using Core.Application.Usuarios.Queries.BuscarUsuarioPorId;
using MahApps.Metro.Controls.Dialogs;
using MediatR;
using Presentation.WpfApp.ViewModels.Empresas;
using Presentation.WpfApp.ViewModels.Roles;

namespace Presentation.WpfApp.ViewModels.Usuarios
{
    public sealed class EditarUsuarioViewModel : Screen
    {
        private readonly IDialogCoordinator _dialogCoordinator;
        private readonly IMediator _mediator;
        private readonly IWindowManager _windowManager;
        private string _apellido;
        private string _contrasena;
        private string _email;
        private EmpresaPerfilDto _empresaPermitidaSeleccionada;
        private string _nombreUsuario;
        private string _primerNombre;
        private RolDto _rolSeleccionado;
        private int _usuarioId;

        public EditarUsuarioViewModel(IMediator mediator, IWindowManager windowManager, IDialogCoordinator dialogCoordinator)
        {
            _mediator = mediator;
            _windowManager = windowManager;
            _dialogCoordinator = dialogCoordinator;
            DisplayName = "Editar Usuario";
        }

        public int UsuarioId
        {
            get => _usuarioId;
            set
            {
                if (value == _usuarioId)
                {
                    return;
                }

                _usuarioId = value;
                NotifyOfPropertyChange(() => UsuarioId);
            }
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
                RaiseGuards();
            }
        }

        public BindableCollection<RolDto> Roles { get; } = new BindableCollection<RolDto>();

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

        public BindableCollection<EmpresaPerfilDto> EmpresasPermitidas { get; } = new BindableCollection<EmpresaPerfilDto>();

        public EmpresaPerfilDto EmpresaPermitidaSeleccionada
        {
            get => _empresaPermitidaSeleccionada;
            set
            {
                if (_empresaPermitidaSeleccionada == value)
                {
                    return;
                }

                _empresaPermitidaSeleccionada = value;
                NotifyOfPropertyChange(() => EmpresaPermitidaSeleccionada);
                RaiseGuards();
            }
        }

        public bool CanRemoverRolAsync => RolSeleccionado != null;

        public bool CanCambiarContrasenaAsync => !string.IsNullOrWhiteSpace(Contrasena);

        public bool CanAgregarEmpresaPermitidaAsync => UsuarioId != 0;

        public bool CanRemoverEmpresaPermitidaAsync => UsuarioId != 0 && EmpresaPermitidaSeleccionada != null;

        public async Task InicializarAsync(int id)
        {
            var usuario = await _mediator.Send(new BuscarUsuarioPorIdQuery(id));

            UsuarioId = usuario.Id;
            PrimerNombre = usuario.PrimerNombre;
            Apellido = usuario.Apellido;
            Email = usuario.Email;
            NombreUsuario = usuario.NombreUsuario;

            Roles.Clear();
            Roles.AddRange(usuario.Roles);

            EmpresasPermitidas.Clear();
            EmpresasPermitidas.AddRange(await _mediator.Send(new BuscarEmpresasPermitidasPorUsuarioQuery(UsuarioId)));
        }

        public async Task ActualizarPerfilAsync()
        {
            try
            {
                await _mediator.Send(new ActualizarPerfilUsuarioCommand(UsuarioId, PrimerNombre, Apellido, Email, NombreUsuario));
                await _dialogCoordinator.ShowMessageAsync(this, "Perfil Actualizado", "El perfil del usuario fue actualizado exitosamente.");
            }
            catch (Exception e)
            {
                await _dialogCoordinator.ShowMessageAsync(this, "Error", e.ToString());
            }
        }

        public async Task CambiarContrasenaAsync()
        {
            try
            {
                await _mediator.Send(new CambiarContrasenaCommand(UsuarioId, Contrasena));
                await _dialogCoordinator.ShowMessageAsync(this, "Contrasena Actualizada", "La contrasena se cambio exitosamete.");
            }
            catch (Exception e)
            {
                await _dialogCoordinator.ShowMessageAsync(this, "Error", e.ToString());
            }
        }

        public async Task AgregarRolAsync()
        {
            try
            {
                var roles = await _mediator.Send(new BuscarRolesQuery());
                var rolesUnicos = roles.Where(pa => Roles.All(p => p.Id != pa.Id)).ToList();
                var viewModel = IoC.Get<SeleccionarRolViewModel>();
                viewModel.Inicializar(rolesUnicos);
                await _windowManager.ShowDialogAsync(viewModel);
                if (viewModel.SeleccionoRol)
                {
                    await _mediator.Send(new AgregarRolCommand(UsuarioId, viewModel.RolSeleccionado.Id));
                    await InicializarAsync(UsuarioId);
                    await _dialogCoordinator.ShowMessageAsync(this, "Rol Agregado", "Rol agregado exitosamente.");
                }
            }
            catch (Exception e)
            {
                await _dialogCoordinator.ShowMessageAsync(this, "Error", e.ToString());
            }
        }

        public async Task RemoverRolAsync()
        {
            try
            {
                await _mediator.Send(new RemoverRolCommand(UsuarioId, RolSeleccionado.Id));
                await InicializarAsync(UsuarioId);
                await _dialogCoordinator.ShowMessageAsync(this, "Rol Removido", "Rol removido exitosamente.");
            }
            catch (Exception e)
            {
                await _dialogCoordinator.ShowMessageAsync(this, "Error", e.ToString());
            }
        }

        public async Task AgregarEmpresaPermitidaAsync()
        {
            try
            {
                var empresaPermitidas = await _mediator.Send(new BuscarEmpresasPermitidasPorUsuarioQuery(UsuarioId));
                var empresas = await _mediator.Send(new BuscarEmpresasQuery());

                var empresasDisponibles = empresas.Where(e => EmpresasPermitidas.All(ep => ep.Id != e.Id)).ToList();

                var seleccionarEmpresaViewModel = IoC.Get<SeleccionarEmpresaViewModel>();
                seleccionarEmpresaViewModel.Inicializar(empresasDisponibles);

                await _windowManager.ShowDialogAsync(seleccionarEmpresaViewModel);
                if (seleccionarEmpresaViewModel.SeleccionoEmpresa)
                {
                    await _mediator.Send(new AgregarEmpresaPermitidaCommand(UsuarioId, seleccionarEmpresaViewModel.EmpresaSeleccionada.Id));
                    await InicializarAsync(UsuarioId);
                    await _dialogCoordinator.ShowMessageAsync(this, "Empresa Permitida Agregada", "Empresa permitida agregada exitosamente.");
                }
            }
            catch (Exception e)
            {
                await _dialogCoordinator.ShowMessageAsync(this, "Erro", e.ToString());
            }
        }

        public async Task RemoverEmpresaPermitidaAsync()
        {
            try
            {
                await _mediator.Send(new RemoverEmpresaPermitidaCommand(UsuarioId, EmpresaPermitidaSeleccionada.Id));
                await InicializarAsync(UsuarioId);
                await _dialogCoordinator.ShowMessageAsync(this, "Empresa Permitida Removida", "Empresa permitida removida exitosamente.");
            }
            catch (Exception e)
            {
                await _dialogCoordinator.ShowMessageAsync(this, "Erro", e.ToString());
            }
        }

        private void RaiseGuards()
        {
            NotifyOfPropertyChange(() => CanRemoverRolAsync);
            NotifyOfPropertyChange(() => CanCambiarContrasenaAsync);
            NotifyOfPropertyChange(() => CanAgregarEmpresaPermitidaAsync);
            NotifyOfPropertyChange(() => CanRemoverEmpresaPermitidaAsync);
        }
    }
}