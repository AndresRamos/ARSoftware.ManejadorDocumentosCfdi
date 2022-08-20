using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Data;
using Caliburn.Micro;
using Core.Application.Empresas.Models;
using Core.Application.Empresas.Queries.BuscarEmpresas;
using MediatR;

namespace Presentation.WpfApp.ViewModels.Empresas
{
    public sealed class SeleccionarEmpresaViewModel : Screen
    {
        private readonly IMediator _mediator;
        private EmpresaPerfilDto _empresaSeleccionada;
        private string _filtro;

        public SeleccionarEmpresaViewModel(IMediator mediator)
        {
            _mediator = mediator;
            DisplayName = "Seleccionar Empresa";
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

        public BindableCollection<EmpresaPerfilDto> Empresas { get; } = new BindableCollection<EmpresaPerfilDto>();

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

        public bool SeleccionoEmpresa { get; private set; }

        public bool CanSeleccionarAsync => EmpresaSeleccionada != null;

        public async Task InicializarAsync()
        {
            await CargarEmpresasAsync();
        }

        public void Inicializar(IEnumerable<EmpresaPerfilDto> empresasDisponibles)
        {
            Empresas.Clear();
            Empresas.AddRange(empresasDisponibles);
        }

        private async Task CargarEmpresasAsync()
        {
            Empresas.Clear();
            Empresas.AddRange(await _mediator.Send(new BuscarEmpresasQuery()));
        }

        public async Task SeleccionarAsync()
        {
            SeleccionoEmpresa = true;
            await TryCloseAsync();
        }

        public async Task CancelarAsync()
        {
            SeleccionoEmpresa = false;
            await TryCloseAsync();
        }

        private void RaiseGuards()
        {
            NotifyOfPropertyChange(() => CanSeleccionarAsync);
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
}
