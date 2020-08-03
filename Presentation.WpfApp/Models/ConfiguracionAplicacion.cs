using System.Threading.Tasks;
using Caliburn.Micro;
using Core.Application.ConfiguracionGeneral.Models;
using Core.Application.ConfiguracionGeneral.Queries.BuscarConfiguracionGeneral;
using Core.Application.Empresas.Models;
using Core.Application.Usuarios.Models;
using MediatR;

namespace Presentation.WpfApp.Models
{
    public class ConfiguracionAplicacion : PropertyChangedBase
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
                NotifyOfPropertyChange(() => Empresa);
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
                NotifyOfPropertyChange(() => ConfiguracionGeneral);
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
                NotifyOfPropertyChange(() => Usuario);
                NotifyOfPropertyChange(() => IsUsuarioAutenticado);
            }
        }

        public bool IsUsuarioAutenticado => Usuario != null;

        public bool IsEmpresaAbierta => Empresa != null;

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
    }
}