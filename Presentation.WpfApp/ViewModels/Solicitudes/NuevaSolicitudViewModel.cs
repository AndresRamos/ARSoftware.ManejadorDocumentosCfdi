using System;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using Core.Application.ConfiguracionGeneral.Queries.BuscarConfiguracionGeneral;
using Core.Application.Rfcs.Queries.BuscarRfcsComercial;
using Core.Application.Rfcs.Queries.BuscarRfcsContabilidad;
using Core.Application.Solicitudes.Commands.CrearSolicitud;
using Core.Application.TiposSolicitud.Models;
using Core.Application.TiposSolicitud.Queries.BuscarTiposSolicitud;
using MahApps.Metro.Controls.Dialogs;
using MediatR;
using Presentation.WpfApp.ViewModels.Rfcs;

namespace Presentation.WpfApp.ViewModels.Solicitudes
{
    public sealed class NuevaSolicitudViewModel : Screen
    {
        private readonly IDialogCoordinator _dialogCoordinator;
        private readonly IMediator _mediator;

        private readonly IWindowManager _windowManager;
        private DateTime _fechaFin = DateTime.Today;
        private DateTime _fechaInicio = DateTime.Today;
        private string _rfcEmisor;
        private string _rfcReceptor;
        private string _rfcSolicitante;
        private TipoSolicitudDto _tipoSolicitudSeleccionado;

        public NuevaSolicitudViewModel(IMediator mediator, IDialogCoordinator dialogCoordinator, IWindowManager windowManager)
        {
            _mediator = mediator;
            _dialogCoordinator = dialogCoordinator;
            _windowManager = windowManager;
            DisplayName = "Nueva Solicitud";
        }

        public DateTime FechaInicio
        {
            get => _fechaInicio;
            set
            {
                if (value.Equals(_fechaInicio))
                {
                    return;
                }

                _fechaInicio = value;
                NotifyOfPropertyChange(() => FechaInicio);
            }
        }

        public DateTime FechaFin
        {
            get => _fechaFin;
            set
            {
                if (value.Equals(_fechaFin))
                {
                    return;
                }

                _fechaFin = value;
                NotifyOfPropertyChange(() => FechaFin);
            }
        }

        public string RfcEmisor
        {
            get => _rfcEmisor;
            set
            {
                if (value == _rfcEmisor)
                {
                    return;
                }

                _rfcEmisor = value;
                NotifyOfPropertyChange(() => RfcEmisor);
            }
        }

        public string RfcReceptor
        {
            get => _rfcReceptor;
            set
            {
                if (value == _rfcReceptor)
                {
                    return;
                }

                _rfcReceptor = value;
                NotifyOfPropertyChange(() => RfcReceptor);
            }
        }

        public string RfcSolicitante
        {
            get => _rfcSolicitante;
            set
            {
                if (value == _rfcSolicitante)
                {
                    return;
                }

                _rfcSolicitante = value;
                NotifyOfPropertyChange(() => RfcSolicitante);
            }
        }

        public BindableCollection<TipoSolicitudDto> TiposSolicitud { get; } = new BindableCollection<TipoSolicitudDto>();

        public TipoSolicitudDto TipoSolicitudSeleccionado
        {
            get => _tipoSolicitudSeleccionado;
            set
            {
                if (value == _tipoSolicitudSeleccionado)
                {
                    return;
                }

                _tipoSolicitudSeleccionado = value;
                NotifyOfPropertyChange(() => TipoSolicitudSeleccionado);
            }
        }

        public async Task InicializarAsync()
        {
            var configuracionGeneral = await _mediator.Send(new BuscarConfiguracionGeneralQuery());
            RfcReceptor = configuracionGeneral.CertificadoSat.Rfc;
            RfcSolicitante = configuracionGeneral.CertificadoSat.Rfc;
            await CargarTiposSolicitudAsync();
        }

        private async Task CargarTiposSolicitudAsync()
        {
            TiposSolicitud.Clear();
            TiposSolicitud.AddRange(await _mediator.Send(new BuscarTiposSolicitudQuery()));
            TipoSolicitudSeleccionado = TiposSolicitud.FirstOrDefault();
        }

        public async Task CrearSolicitudAsync()
        {
            try
            {
                await _mediator.Send(new CrearSolicitudCommand(FechaInicio, FechaFin, RfcEmisor, RfcReceptor, RfcSolicitante, TipoSolicitudSeleccionado.Name));
                await TryCloseAsync();
            }
            catch (Exception e)
            {
                await _dialogCoordinator.ShowMessageAsync(this, "Error", e.ToString());
            }
        }

        public async Task BuscarRfcEmisorComercialAsync()
        {
            try
            {
                var viewModel = IoC.Get<SeleccionarRfcViewModel>();
                viewModel.Inicializar(await _mediator.Send(new BuscarRfcsComercialQuery()));
                await _windowManager.ShowDialogAsync(viewModel);
                if (viewModel.SeleccionoRfc)
                {
                    RfcEmisor = viewModel.RfcSeleccionado.Rfc;
                }
            }
            catch (Exception e)
            {
                await _dialogCoordinator.ShowMessageAsync(this, "Error", e.ToString());
            }
        }

        public async Task BuscarRfcReceptorComercialAsync()
        {
            try
            {
                var viewModel = IoC.Get<SeleccionarRfcViewModel>();
                viewModel.Inicializar(await _mediator.Send(new BuscarRfcsComercialQuery()));
                await _windowManager.ShowDialogAsync(viewModel);
                if (viewModel.SeleccionoRfc)
                {
                    RfcReceptor = viewModel.RfcSeleccionado.Rfc;
                }
            }
            catch (Exception e)
            {
                await _dialogCoordinator.ShowMessageAsync(this, "Error", e.ToString());
            }
        }

        public async Task BuscarRfcEmisorContabilidadAsync()
        {
            try
            {
                var viewModel = IoC.Get<SeleccionarRfcViewModel>();
                viewModel.Inicializar(await _mediator.Send(new BuscarRfcsContabilidadQuery()));
                await _windowManager.ShowDialogAsync(viewModel);
                if (viewModel.SeleccionoRfc)
                {
                    RfcEmisor = viewModel.RfcSeleccionado.Rfc;
                }
            }
            catch (Exception e)
            {
                await _dialogCoordinator.ShowMessageAsync(this, "Error", e.ToString());
            }
        }

        public async Task BuscarRfcReceptorContabilidadAsync()
        {
            try
            {
                var viewModel = IoC.Get<SeleccionarRfcViewModel>();
                viewModel.Inicializar(await _mediator.Send(new BuscarRfcsContabilidadQuery()));
                await _windowManager.ShowDialogAsync(viewModel);
                if (viewModel.SeleccionoRfc)
                {
                    RfcReceptor = viewModel.RfcSeleccionado.Rfc;
                }
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