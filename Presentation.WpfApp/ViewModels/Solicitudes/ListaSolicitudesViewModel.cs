using System.ComponentModel;
using System.Windows.Data;
using Caliburn.Micro;
using Common.Models;
using Core.Application.Common;
using Core.Application.Solicitudes.Commands.ProcesarSolicitud;
using Core.Application.Solicitudes.Models;
using Core.Application.Solicitudes.Queries.BuscarSolicitudesPorRangoFecha;
using MahApps.Metro.Controls.Dialogs;
using MediatR;

namespace Presentation.WpfApp.ViewModels.Solicitudes;

public sealed class ListaSolicitudesViewModel : Screen
{
    private readonly ConfiguracionAplicacion _configuracionAplicacion;
    private readonly IDialogCoordinator _dialogCoordinator;
    private readonly IMediator _mediator;
    private readonly IWindowManager _windowManager;
    private DateTime _fechaFin = DateTime.Today;
    private DateTime _fechaInicio = DateTime.Today;
    private string _filtro;
    private SolicitudDto _solicitudSeleccionada;

    public ListaSolicitudesViewModel(IMediator mediator,
                                     IWindowManager windowManager,
                                     IDialogCoordinator dialogCoordinator,
                                     ConfiguracionAplicacion configuracionAplicacion)
    {
        _mediator = mediator;
        _windowManager = windowManager;
        _dialogCoordinator = dialogCoordinator;
        _configuracionAplicacion = configuracionAplicacion;
        DisplayName = "Lista De Solicitudes";
        SolicitudesView = CollectionViewSource.GetDefaultView(Solicitudes);
        SolicitudesView.Filter = SolicitudesView_Filter;
    }

    public string Filtro
    {
        get => _filtro;
        set
        {
            if (value == _filtro)
                return;

            _filtro = value;
            NotifyOfPropertyChange(() => Filtro);
            SolicitudesView.Refresh();
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
            RaiseGuards();
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
            RaiseGuards();
        }
    }

    public BindableCollection<SolicitudDto> Solicitudes { get; } = new();

    public ICollectionView SolicitudesView { get; }

    public SolicitudDto SolicitudSeleccionada
    {
        get => _solicitudSeleccionada;
        set
        {
            if (Equals(value, _solicitudSeleccionada))
                return;

            _solicitudSeleccionada = value;
            NotifyOfPropertyChange(() => SolicitudSeleccionada);
            RaiseGuards();
        }
    }

    public bool CanBuscarSolicitudesAsync => FechaInicio <= FechaFin;

    public bool CanProcesarSolicitudAsync =>
        SolicitudSeleccionada != null &&
        _configuracionAplicacion.IsUsuarioAutenticado &&
        _configuracionAplicacion.Usuario.TienePermiso(PermisosAplicacion.PuedeProcesarSolicitud);

    public bool CanVerDetallesSolicitudSeleccionadaAsync =>
        SolicitudSeleccionada != null &&
        _configuracionAplicacion.IsUsuarioAutenticado &&
        _configuracionAplicacion.Usuario.TienePermiso(PermisosAplicacion.PuedeProcesarSolicitud);

    public bool CanCrearNuevaSolicitudAsync =>
        _configuracionAplicacion.IsUsuarioAutenticado &&
        _configuracionAplicacion.Usuario.TienePermiso(PermisosAplicacion.PuedeCrearSolicitud);

    public async Task BuscarSolicitudesAsync()
    {
        ProgressDialogController progressDialogController =
            await _dialogCoordinator.ShowProgressAsync(this, "Buscando Solicitudes", "Buscando solicitudes.");
        progressDialogController.SetIndeterminate();
        await Task.Delay(1000);

        try
        {
            Solicitudes.Clear();
            Solicitudes.AddRange(await _mediator.Send(new BuscarSolicitudesPorRangoFechaQuery(FechaInicio, FechaFin)));
        }
        catch (Exception e)
        {
            await _dialogCoordinator.ShowMessageAsync(this, "Error", e.ToString());
        }
        finally
        {
            RaiseGuards();
        }

        await progressDialogController.CloseAsync();
    }

    public async Task CrearNuevaSolicitudAsync()
    {
        try
        {
            var viewModel = IoC.Get<NuevaSolicitudViewModel>();
            await viewModel.InicializarAsync();
            await _windowManager.ShowDialogAsync(viewModel);
        }
        catch (Exception e)
        {
            await _dialogCoordinator.ShowMessageAsync(this, "Error", e.ToString());
        }
        finally
        {
            await BuscarSolicitudesAsync();
            RaiseGuards();
        }
    }

    public async Task ProcesarSolicitudAsync()
    {
        int solicitudId = SolicitudSeleccionada.Id;

        MessageDialogResult messageDialogResult = await _dialogCoordinator.ShowMessageAsync(this,
            "Procesar Solicitud",
            $"Esta seguro de querer procesar la solicitud {SolicitudSeleccionada.Id}?",
            MessageDialogStyle.AffirmativeAndNegative,
            new MetroDialogSettings { AffirmativeButtonText = "Si", NegativeButtonText = "No" });
        if (messageDialogResult != MessageDialogResult.Affirmative)
            return;

        using var cancellationTokenSource = new CancellationTokenSource();

        ProgressDialogController progressDialogController = await _dialogCoordinator.ShowProgressAsync(this,
            "Procesando Solicitud",
            "Procesando solicitud. Este proceso puede tardar hasta 5 minutos.",
            true);
        progressDialogController.Canceled += (_, _) =>
        {
            // ReSharper disable once AccessToDisposedClosure
            cancellationTokenSource.Cancel();
        };
        progressDialogController.SetIndeterminate();
        await Task.Delay(1000);

        try
        {
            await _mediator.Send(new ProcesarSolicitudCommand(solicitudId, _configuracionAplicacion.Usuario.Id),
                cancellationTokenSource.Token);
            var viewModel = IoC.Get<DetalleSolicitudViewModel>();
            await viewModel.InicializarAsync(solicitudId);
            await _windowManager.ShowWindowAsync(viewModel);
            await BuscarSolicitudesAsync();
            SolicitudSeleccionada = Solicitudes.FirstOrDefault(s => s.Id == solicitudId);
        }
        catch (Exception e)
        {
            await _dialogCoordinator.ShowMessageAsync(this, "Error", e.ToString());
        }
        finally
        {
            RaiseGuards();
            await progressDialogController.CloseAsync();
        }
    }

    public async Task VerDetallesSolicitudSeleccionadaAsync()
    {
        try
        {
            var viewModel = IoC.Get<DetalleSolicitudViewModel>();
            await viewModel.InicializarAsync(SolicitudSeleccionada.Id);
            await _windowManager.ShowWindowAsync(viewModel);
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
        NotifyOfPropertyChange(() => CanBuscarSolicitudesAsync);
        NotifyOfPropertyChange(() => CanCrearNuevaSolicitudAsync);
        NotifyOfPropertyChange(() => CanProcesarSolicitudAsync);
        NotifyOfPropertyChange(() => CanVerDetallesSolicitudSeleccionadaAsync);
    }

    private bool SolicitudesView_Filter(object obj)
    {
        if (!(obj is SolicitudDto solicitud))
            throw new ArgumentNullException(nameof(obj));

        return string.IsNullOrEmpty(Filtro) ||
               solicitud.Id.ToString().IndexOf(Filtro, StringComparison.OrdinalIgnoreCase) >= 0 ||
               solicitud.RfcEmisor?.IndexOf(Filtro, StringComparison.OrdinalIgnoreCase) >= 0 ||
               solicitud.RfcReceptor?.IndexOf(Filtro, StringComparison.OrdinalIgnoreCase) >= 0 ||
               solicitud.RfcSolicitante?.IndexOf(Filtro, StringComparison.OrdinalIgnoreCase) >= 0 ||
               solicitud.TipoSolicitud?.IndexOf(Filtro, StringComparison.OrdinalIgnoreCase) >= 0;
    }
}
