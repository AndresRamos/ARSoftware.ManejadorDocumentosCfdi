using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Caliburn.Micro;
using Core.Application.Solicitudes.Commands.VerificarSolicitud;
using Core.Application.Solicitudes.Models;
using Core.Application.Solicitudes.Queries.BuscarSolicitudPorId;
using MahApps.Metro.Controls.Dialogs;
using MediatR;
using Presentation.WpfApp.ViewModels.Xmls;

namespace Presentation.WpfApp.ViewModels.Solicitudes
{
    public class SolicitudVerificacionViewModel : Screen
    {
        private readonly IDialogCoordinator _dialogCoordinator;

        private readonly IMediator _mediator;
        private readonly IWindowManager _windowManager;
        private SolicitudVerificacionDto _solicitud;
        private SolicitudVerificacionDto _solicitudSeleccionada;

        public SolicitudVerificacionViewModel(IDialogCoordinator dialogCoordinator, IWindowManager windowManager, IMediator mediator)
        {
            _dialogCoordinator = dialogCoordinator;
            _windowManager = windowManager;
            _mediator = mediator;
            DisplayName = "Solicitud Verificacion";
        }

        public SolicitudVerificacionDto Solicitud
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

        public BindableCollection<SolicitudVerificacionDto> Solicitudes { get; } = new BindableCollection<SolicitudVerificacionDto>();

        public SolicitudVerificacionDto SolicitudSeleccionada
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

        private int SolicitudId { get; set; }

        public void Inicializar(int solicitudId, SolicitudVerificacionDto solicitud, IEnumerable<SolicitudVerificacionDto> solicitudes)
        {
            SolicitudId = solicitudId;
            Solicitud = solicitud;
            Solicitudes.Clear();
            Solicitudes.AddRange(solicitudes);
        }

        public async Task EnviarSolicitudAsync()
        {
            var progressDialogController = await _dialogCoordinator.ShowProgressAsync(this, "Enviando Solicitud", "Enviando solicitud");
            progressDialogController.SetIndeterminate();
            await Task.Delay(1000);

            try
            {
                await _mediator.Send(new VerificarSolicitudCommand(SolicitudId));
                Solicitud = (await _mediator.Send(new BuscarSolicitudPorIdQuery(SolicitudId))).SolicitudVerificacion;
            }
            catch (Exception e)
            {
                await _dialogCoordinator.ShowMessageAsync(this, "Error", e.ToString());
            }

            await progressDialogController.CloseAsync();
        }

        public async Task VerSolicitudXmlAsync(SolicitudVerificacionDto solicitud)
        {
            try
            {
                var viewModel = IoC.Get<XmlViewerViewModel>();
                viewModel.Inicializar(solicitud.Solicitud);
                _windowManager.ShowDialog(viewModel);
            }
            catch (Exception e)
            {
                await _dialogCoordinator.ShowMessageAsync(this, "Error", e.ToString());
            }
        }

        public bool CanVerSolicitudXmlAsync(SolicitudVerificacionDto solicitud)
        {
            return !string.IsNullOrEmpty(solicitud?.Solicitud);
        }

        public async Task VerRespuestaXmlAsync(SolicitudVerificacionDto solicitud)
        {
            try
            {
                var viewModel = IoC.Get<XmlViewerViewModel>();
                viewModel.Inicializar(solicitud.Respuesta);
                _windowManager.ShowDialog(viewModel);
            }
            catch (Exception e)
            {
                await _dialogCoordinator.ShowMessageAsync(this, "Error", e.ToString());
            }
        }

        public bool CanVerRespuestaXmlAsync(SolicitudVerificacionDto solicitud)
        {
            return !string.IsNullOrEmpty(solicitud?.Respuesta);
        }
    }
}