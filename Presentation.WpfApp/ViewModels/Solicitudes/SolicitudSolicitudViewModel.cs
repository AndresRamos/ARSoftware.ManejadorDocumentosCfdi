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

namespace Presentation.WpfApp.ViewModels.Solicitudes
{
    public sealed class SolicitudSolicitudViewModel : Screen
    {
        private readonly IDialogCoordinator _dialogCoordinator;
        private readonly IMediator _mediator;
        private readonly IWindowManager _windowManager;
        private SolicitudSolicitudDto _solicitud;
        private SolicitudSolicitudDto _solicitudSeleccionada;

        public SolicitudSolicitudViewModel(IDialogCoordinator dialogCoordinator, IWindowManager windowManager, IMediator mediator)
        {
            _dialogCoordinator = dialogCoordinator;
            _windowManager = windowManager;
            _mediator = mediator;
            DisplayName = "Solicitud Solicitud";
        }

        public SolicitudSolicitudDto Solicitud
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

        public BindableCollection<SolicitudSolicitudDto> Solicitudes { get; } = new BindableCollection<SolicitudSolicitudDto>();

        public SolicitudSolicitudDto SolicitudSeleccionada
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

        public void Inicializar(int solicitudId, SolicitudSolicitudDto solicitud, IEnumerable<SolicitudSolicitudDto> solicitudes)
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
                await _mediator.Send(new AutenticarSolicitudCommand(SolicitudId));
                Solicitud = (await _mediator.Send(new BuscarSolicitudPorIdQuery(SolicitudId))).SolicitudSolicitud;
            }
            catch (Exception e)
            {
                await _dialogCoordinator.ShowMessageAsync(this, "Error", e.ToString());
            }

            await progressDialogController.CloseAsync();
        }

        public async Task VerSolicitudXmlAsync(SolicitudSolicitudDto solicitud)
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

        public bool CanVerSolicitudXmlAsync(SolicitudSolicitudDto solicitud)
        {
            return !string.IsNullOrEmpty(solicitud?.Solicitud);
        }

        public async Task VerRespuestaXmlAsync(SolicitudSolicitudDto solicitud)
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

        public bool CanVerRespuestaXmlAsync(SolicitudSolicitudDto solicitud)
        {
            return !string.IsNullOrEmpty(solicitud?.Respuesta);
        }
    }
}