using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using Common.DateRanges;
using Core.Application.Rfcs.Queries.BuscarRfcsComercial;
using Core.Application.Rfcs.Queries.BuscarRfcsContabilidad;
using Core.Application.Solicitudes.Commands.CrearSolicitud;
using Core.Application.TiposSolicitud.Models;
using Core.Application.TiposSolicitud.Queries.BuscarTiposSolicitud;
using MahApps.Metro.Controls.Dialogs;
using MediatR;
using Presentation.WpfApp.Models;
using Presentation.WpfApp.ViewModels.Rfcs;

namespace Presentation.WpfApp.ViewModels.Solicitudes
{
    public sealed class NuevaSolicitudViewModel : Screen
    {
        private readonly ConfiguracionAplicacion _configuracionAplicacion;
        private readonly IDialogCoordinator _dialogCoordinator;
        private readonly IMediator _mediator;
        private readonly IWindowManager _windowManager;
        private DateTime _fechaFin;
        private DateTime _fechaInicio;
        private string _rfcEmisor;
        private string _rfcReceptor;
        private string _rfcSolicitante;
        private TipoRangoFechaEnum _tipoRangoFechaSeleccionado;
        private TipoSolicitudDto _tipoSolicitudSeleccionado;

        public NuevaSolicitudViewModel(ConfiguracionAplicacion configuracionAplicacion, IMediator mediator, IDialogCoordinator dialogCoordinator, IWindowManager windowManager)
        {
            _configuracionAplicacion = configuracionAplicacion;
            _mediator = mediator;
            _dialogCoordinator = dialogCoordinator;
            _windowManager = windowManager;
            DisplayName = "Nueva Solicitud";
            TipoRangoFechaSeleccionado = TipoRangoFechaEnum.Hoy;
        }

        public IEnumerable<TipoRangoFechaEnum> TiposRangoFecha => Enum.GetValues(typeof(TipoRangoFechaEnum)).Cast<TipoRangoFechaEnum>();

        public TipoRangoFechaEnum TipoRangoFechaSeleccionado
        {
            get => _tipoRangoFechaSeleccionado;
            set
            {
                if (_tipoRangoFechaSeleccionado == value)
                {
                    return;
                }

                _tipoRangoFechaSeleccionado = value;
                NotifyOfPropertyChange(() => TipoRangoFechaSeleccionado);

                switch (TipoRangoFechaSeleccionado)
                {
                    case TipoRangoFechaEnum.Custumizado:
                        break;
                    case TipoRangoFechaEnum.Hoy:
                        FechaInicio = RangoFecha.Hoy.Inicio;
                        FechaFin = RangoFecha.Hoy.Fin;
                        break;
                    case TipoRangoFechaEnum.Ayer:
                        FechaInicio = RangoFecha.Ayer.Inicio;
                        FechaFin = RangoFecha.Ayer.Fin;
                        break;
                    case TipoRangoFechaEnum.EstaSemana:
                        FechaInicio = RangoFecha.EstaSemana.Inicio;
                        FechaFin = RangoFecha.EstaSemana.Fin;
                        break;
                    case TipoRangoFechaEnum.EstaSemanaAlDia:
                        FechaInicio = RangoFecha.EstaSemanaAlDia.Inicio;
                        FechaFin = RangoFecha.EstaSemanaAlDia.Fin;
                        break;
                    case TipoRangoFechaEnum.EsteMes:
                        FechaInicio = RangoFecha.EsteMes.Inicio;
                        FechaFin = RangoFecha.EsteMes.Fin;
                        break;
                    case TipoRangoFechaEnum.EsteMesAlDia:
                        FechaInicio = RangoFecha.EsteMesAlDia.Inicio;
                        FechaFin = RangoFecha.EsteMesAlDia.Fin;
                        break;
                    case TipoRangoFechaEnum.EsteAno:
                        FechaInicio = RangoFecha.EsteAno.Inicio;
                        FechaFin = RangoFecha.EsteAno.Fin;
                        break;
                    case TipoRangoFechaEnum.EsteAnoAlDia:
                        FechaInicio = RangoFecha.EsteAnoAlDia.Inicio;
                        FechaFin = RangoFecha.EsteAnoAlDia.Fin;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
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

                if (TipoRangoFechaSeleccionado != TipoRangoFechaEnum.Custumizado)
                {
                    TipoRangoFechaSeleccionado = TipoRangoFechaEnum.Custumizado;
                }
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

                if (TipoRangoFechaSeleccionado != TipoRangoFechaEnum.Custumizado)
                {
                    TipoRangoFechaSeleccionado = TipoRangoFechaEnum.Custumizado;
                }


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
            RfcReceptor = _configuracionAplicacion.ConfiguracionGeneral.CertificadoSat.Rfc;
            RfcSolicitante = _configuracionAplicacion.ConfiguracionGeneral.CertificadoSat.Rfc;
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
                await _mediator.Send(new CrearSolicitudCommand(_configuracionAplicacion.Empresa.Id, _configuracionAplicacion.Usuario.Id, FechaInicio, FechaFin, RfcEmisor, RfcReceptor, RfcSolicitante, TipoSolicitudSeleccionado.Name));
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