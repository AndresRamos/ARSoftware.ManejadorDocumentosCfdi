using Caliburn.Micro;
using Common.DateRanges;
using Core.Application.Common;
using Core.Application.Rfcs.Queries.BuscarRfcsComercial;
using Core.Application.Rfcs.Queries.BuscarRfcsContabilidad;
using Core.Application.Solicitudes.Commands.CrearSolicitud;
using Core.Application.TiposSolicitud.Models;
using Core.Application.TiposSolicitud.Queries.BuscarTiposSolicitud;
using MahApps.Metro.Controls.Dialogs;
using MediatR;
using Presentation.WpfApp.ViewModels.Rfcs;

namespace Presentation.WpfApp.ViewModels.Solicitudes;

public sealed class NuevaSolicitudViewModel : Screen
{
    private readonly ConfiguracionAplicacion _configuracionAplicacion;
    private readonly IDialogCoordinator _dialogCoordinator;
    private readonly IMediator _mediator;
    private readonly IWindowManager _windowManager;
    private DateTime _fechaFin;
    private DateTime _fechaInicio;
    private string _rfcEmisor = string.Empty;
    private string _rfcReceptor = string.Empty;
    private string _rfcSolicitante = string.Empty;
    private TipoRangoFechaEnum _tipoRangoFechaSeleccionado;
    private TipoSolicitudDto _tipoSolicitudSeleccionado;
    private string _uuid;

    public NuevaSolicitudViewModel(ConfiguracionAplicacion configuracionAplicacion,
                                   IMediator mediator,
                                   IDialogCoordinator dialogCoordinator,
                                   IWindowManager windowManager)
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
                return;

            _tipoRangoFechaSeleccionado = value;
            NotifyOfPropertyChange(() => TipoRangoFechaSeleccionado);

            switch (TipoRangoFechaSeleccionado)
            {
                case TipoRangoFechaEnum.Custumizado:
                    break;
                case TipoRangoFechaEnum.Hoy:
                    _fechaInicio = RangoFecha.Hoy.Inicio;
                    _fechaFin = RangoFecha.Hoy.Fin;
                    break;
                case TipoRangoFechaEnum.Ayer:
                    _fechaInicio = RangoFecha.Ayer.Inicio;
                    _fechaFin = RangoFecha.Ayer.Fin;
                    break;
                case TipoRangoFechaEnum.EstaSemana:
                    _fechaInicio = RangoFecha.EstaSemana.Inicio;
                    _fechaFin = RangoFecha.EstaSemana.Fin;
                    break;
                case TipoRangoFechaEnum.EstaSemanaAlDia:
                    _fechaInicio = RangoFecha.EstaSemanaAlDia.Inicio;
                    _fechaFin = RangoFecha.EstaSemanaAlDia.Fin;
                    break;
                case TipoRangoFechaEnum.EsteMes:
                    _fechaInicio = RangoFecha.EsteMes.Inicio;
                    _fechaFin = RangoFecha.EsteMes.Fin;
                    break;
                case TipoRangoFechaEnum.EsteMesAlDia:
                    _fechaInicio = RangoFecha.EsteMesAlDia.Inicio;
                    _fechaFin = RangoFecha.EsteMesAlDia.Fin;
                    break;
                case TipoRangoFechaEnum.EsteAno:
                    _fechaInicio = RangoFecha.EsteAno.Inicio;
                    _fechaFin = RangoFecha.EsteAno.Fin;
                    break;
                case TipoRangoFechaEnum.EsteAnoAlDia:
                    _fechaInicio = RangoFecha.EsteAnoAlDia.Inicio;
                    _fechaFin = RangoFecha.EsteAnoAlDia.Fin;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            NotifyOfPropertyChange(() => FechaInicio);
            NotifyOfPropertyChange(() => FechaFin);
        }
    }

    public DateTime FechaInicio
    {
        get => _fechaInicio;
        set
        {
            if (value.Equals(_fechaInicio))
                return;

            _fechaInicio = value;
            NotifyOfPropertyChange(() => FechaInicio);

            if (TipoRangoFechaSeleccionado != TipoRangoFechaEnum.Custumizado)
                TipoRangoFechaSeleccionado = TipoRangoFechaEnum.Custumizado;
        }
    }

    public DateTime FechaFin
    {
        get => _fechaFin;
        set
        {
            if (value.Equals(_fechaFin))
                return;

            _fechaFin = value;
            NotifyOfPropertyChange(() => FechaFin);

            if (TipoRangoFechaSeleccionado != TipoRangoFechaEnum.Custumizado)
                TipoRangoFechaSeleccionado = TipoRangoFechaEnum.Custumizado;
        }
    }

    public string RfcEmisor
    {
        get => _rfcEmisor;
        set
        {
            if (value == _rfcEmisor)
                return;

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
                return;

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
                return;

            _rfcSolicitante = value;
            NotifyOfPropertyChange(() => RfcSolicitante);
        }
    }

    public BindableCollection<TipoSolicitudDto> TiposSolicitud { get; } = new();

    public TipoSolicitudDto TipoSolicitudSeleccionado
    {
        get => _tipoSolicitudSeleccionado;
        set
        {
            if (value == _tipoSolicitudSeleccionado)
                return;

            _tipoSolicitudSeleccionado = value;
            NotifyOfPropertyChange(() => TipoSolicitudSeleccionado);
        }
    }

    public string Uuid
    {
        get => _uuid;
        set => Set(ref _uuid, value);
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
            await _mediator.Send(new CrearSolicitudCommand(_configuracionAplicacion.Empresa.Id,
                _configuracionAplicacion.Usuario.Id,
                FechaInicio,
                FechaFin,
                RfcEmisor,
                RfcReceptor,
                RfcSolicitante,
                TipoSolicitudSeleccionado.Name,
                Uuid));
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
                RfcEmisor = viewModel.RfcSeleccionado.Rfc;
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
                RfcReceptor = viewModel.RfcSeleccionado.Rfc;
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
                RfcEmisor = viewModel.RfcSeleccionado.Rfc;
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
                RfcReceptor = viewModel.RfcSeleccionado.Rfc;
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
