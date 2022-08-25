using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Caliburn.Micro;
using Core.Application.Solicitudes.Commands.AutenticarSolicitud;
using Core.Application.Solicitudes.Models;
using Core.Application.Solicitudes.Queries.BuscarSolicitudPorId;
using MahApps.Metro.Controls.Dialogs;
using MediatR;
using Presentation.WpfApp.ViewModels.Xmls;

namespace Presentation.WpfApp.ViewModels.Solicitudes;

public sealed class SolicitudAutenticacionViewModel : Screen
{
    private readonly IDialogCoordinator _dialogCoordinator;

    private readonly IMediator _mediator;
    private readonly IWindowManager _windowManager;
    private SolicitudAutenticacionDto _solicitud;
    private SolicitudAutenticacionDto _solicitudSeleccionada;

    public SolicitudAutenticacionViewModel(IDialogCoordinator dialogCoordinator, IWindowManager windowManager, IMediator mediator)
    {
        _dialogCoordinator = dialogCoordinator;
        _windowManager = windowManager;
        _mediator = mediator;
        DisplayName = "Solicitud Autenticacion";
    }

    private int SolicitudId { get; set; }

    public SolicitudAutenticacionDto Solicitud
    {
        get => _solicitud;
        private set
        {
            if (Equals(value, _solicitud))
            {
                return;
            }

            _solicitud = value;
            NotifyOfPropertyChange(() => Solicitud);
        }
    }

    public BindableCollection<SolicitudAutenticacionDto> Solicitudes { get; } = new();

    public SolicitudAutenticacionDto SolicitudSeleccionada
    {
        get => _solicitudSeleccionada;
        set
        {
            if (Equals(value, _solicitudSeleccionada))
            {
                return;
            }

            _solicitudSeleccionada = value;
            NotifyOfPropertyChange(() => SolicitudSeleccionada);
        }
    }

    public void Inicializar(int solicitudId, SolicitudAutenticacionDto solicitud, IEnumerable<SolicitudAutenticacionDto> solicitudes)
    {
        SolicitudId = solicitudId;
        Solicitud = solicitud;
        Solicitudes.Clear();
        Solicitudes.AddRange(solicitudes);
    }

    public async Task EnviarSolicitudAsync()
    {
        ProgressDialogController progressDialogController =
            await _dialogCoordinator.ShowProgressAsync(this, "Enviando Solicitud", "Enviando solicitud");
        progressDialogController.SetIndeterminate();
        await Task.Delay(1000);

        try
        {
            await _mediator.Send(new AutenticarSolicitudCommand(SolicitudId));
            Solicitud = (await _mediator.Send(new BuscarSolicitudPorIdQuery(SolicitudId))).SolicitudAutenticacion;
        }
        catch (Exception e)
        {
            await _dialogCoordinator.ShowMessageAsync(this, "Error", e.ToString());
        }

        await progressDialogController.CloseAsync();
    }

    public async Task VerSolicitudXmlAsync(SolicitudAutenticacionDto solicitud)
    {
        try
        {
            var viewModel = IoC.Get<XmlViewerViewModel>();
            viewModel.Inicializar(solicitud.Solicitud);
            await _windowManager.ShowDialogAsync(viewModel);
        }
        catch (Exception e)
        {
            await _dialogCoordinator.ShowMessageAsync(this, "Error", e.ToString());
        }
    }

    public bool CanVerSolicitudXmlAsync(SolicitudAutenticacionDto solicitud)
    {
        return !string.IsNullOrEmpty(solicitud?.Solicitud);
    }

    public async Task VerRespuestaXmlAsync(SolicitudAutenticacionDto solicitud)
    {
        try
        {
            var viewModel = IoC.Get<XmlViewerViewModel>();
            viewModel.Inicializar(solicitud.Respuesta);
            await _windowManager.ShowDialogAsync(viewModel);
        }
        catch (Exception e)
        {
            await _dialogCoordinator.ShowMessageAsync(this, "Error", e.ToString());
        }
    }

    public bool CanVerRespuestaXmlAsync(SolicitudAutenticacionDto solicitud)
    {
        return !string.IsNullOrEmpty(solicitud?.Respuesta);
    }
}
