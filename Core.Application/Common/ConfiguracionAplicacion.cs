using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Core.Application.ConfiguracionGeneral.Models;
using Core.Application.ConfiguracionGeneral.Queries.BuscarConfiguracionGeneral;
using Core.Application.Empresas.Models;
using Core.Application.Usuarios.Models;
using MediatR;

namespace Core.Application.Common
{
    public class ConfiguracionAplicacion : INotifyPropertyChanged
    {
        private readonly IMediator _mediator;
        private ConfiguracionGeneralDto _configuracionGeneral;
        private EmpresaPerfilDto _empresa;
        private UsuarioDto _usuario;

        public ConfiguracionAplicacion(IMediator mediator)
        {
            _mediator = mediator;
        }

        public EmpresaPerfilDto Empresa
        {
            get => _empresa;
            private set
            {
                if (_empresa == value)
                {
                    return;
                }

                _empresa = value;
                OnPropertyChanged();
            }
        }

        public ConfiguracionGeneralDto ConfiguracionGeneral
        {
            get => _configuracionGeneral;
            private set
            {
                if (Equals(value, _configuracionGeneral))
                {
                    return;
                }

                _configuracionGeneral = value;
                OnPropertyChanged();
            }
        }

        public UsuarioDto Usuario
        {
            get => _usuario;
            private set
            {
                if (Equals(value, _usuario))
                {
                    return;
                }

                _usuario = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsUsuarioAutenticado));
            }
        }

        public bool IsUsuarioAutenticado => Usuario != null;

        public bool IsEmpresaAbierta => Empresa != null;

        public event PropertyChangedEventHandler PropertyChanged;

        public async Task AbrirEmpresaAsync(EmpresaPerfilDto empresa)
        {
            Empresa = empresa;
            await CargarConfiguracionAsync();
        }

        public async Task CargarConfiguracionAsync()
        {
            ConfiguracionGeneral = await _mediator.Send(new BuscarConfiguracionGeneralQuery(Empresa.Id));
        }

        public void SetUsuario(UsuarioDto usuario)
        {
            Usuario = usuario;
        }

        public void CerrarEmpresa()
        {
            Empresa = null;
            ConfiguracionGeneral = null;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
            {
                return false;
            }

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
