using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Caliburn.Micro;
using Core.Application.Solicitudes.Commands.DescargarSolicitud;
using Core.Application.Solicitudes.Models;
using Core.Application.Solicitudes.Queries.BuscarSolicitudPorId;
using MahApps.Metro.Controls.Dialogs;
using MediatR;
using Presentation.WpfApp.ViewModels.Xmls;

namespace Presentation.WpfApp.ViewModels.Solicitudes
{
    public sealed class SolicitudDescargaViewModel : Screen
    {
        private readonly IDialogCoordinator _dialogCoordinator;
        private readonly IMediator _mediator;
        private readonly IWindowManager _windowManager;
        private SolicitudDescargaDto _solicitud;
        private SolicitudDescargaDto _solicitudSeleccionada;

        public SolicitudDescargaViewModel(IDialogCoordinator dialogCoordinator, IWindowManager windowManager, IMediator mediator)
        {
            _dialogCoordinator = dialogCoordinator;
            _windowManager = windowManager;
            _mediator = mediator;
            DisplayName = "Solicitud Descarga";
        }

        public SolicitudDescargaDto Solicitud
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

        public BindableCollection<SolicitudDescargaDto> Solicitudes { get; } = new BindableCollection<SolicitudDescargaDto>();

        public SolicitudDescargaDto SolicitudSeleccionada
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

        public void Inicializar(int solicitudId, SolicitudDescargaDto solicitud, IEnumerable<SolicitudDescargaDto> solicitudes)
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
                await _mediator.Send(new DescargarSolicitudCommand(SolicitudId));
                Solicitud = (await _mediator.Send(new BuscarSolicitudPorIdQuery(SolicitudId))).SolicitudDescarga;
            }
            catch (Exception e)
            {
                await _dialogCoordinator.ShowMessageAsync(this, "Error", e.ToString());
            }

            await progressDialogController.CloseAsync();
        }

        public async Task VerSolicitudXmlAsync(SolicitudDescargaDto solicitud)
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

        public bool CanVerSolicitudXmlAsync(SolicitudDescargaDto solicitud)
        {
            return !string.IsNullOrEmpty(solicitud?.Solicitud);
        }

        public async Task VerRespuestaXmlAsync(SolicitudDescargaDto solicitud)
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

        public bool CanVerRespuestaXmlAsync(SolicitudDescargaDto solicitud)
        {
            return !string.IsNullOrEmpty(solicitud?.Respuesta);
        }
    }
}
