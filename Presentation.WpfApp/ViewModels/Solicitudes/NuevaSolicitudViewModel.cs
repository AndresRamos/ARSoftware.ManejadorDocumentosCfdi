using System;
using System.IO;
using System.Threading.Tasks;
using Caliburn.Micro;
using Core.Application.Cfdis.Queries.ObtenerCertificado;
using Core.Application.Solicitudes.Commands.CrearSolicitud;
using MahApps.Metro.Controls.Dialogs;
using MediatR;

namespace Presentation.WpfApp.ViewModels.Solicitudes
{
    public sealed class NuevaSolicitudViewModel : Screen
    {
        private readonly IMediator _mediator;
        private readonly IDialogCoordinator _dialogCoordinator;
        private DateTime _fechaInicio = DateTime.Today;
        private DateTime _fechaFin = DateTime.Today;

        public NuevaSolicitudViewModel(IMediator mediator, IDialogCoordinator dialogCoordinator)
        {
            _mediator = mediator;
            _dialogCoordinator = dialogCoordinator;
            DisplayName = "Nueva Solicitud";
        }

        public DateTime FechaInicio
        {
            get => _fechaInicio;
            set
            {
                if (value.Equals(_fechaInicio)) return;
                _fechaInicio = value;
                NotifyOfPropertyChange(() => FechaInicio);
            }
        }

        public DateTime FechaFin
        {
            get => _fechaFin;
            set
            {
                if (value.Equals(_fechaFin)) return;
                _fechaFin = value;
                NotifyOfPropertyChange(() => FechaFin);
            }
        }

        public async Task CrearSolicitudAsync()
        {
            try
            {
                await _mediator.Send(new CrearSolicitudCommand(FechaInicio, FechaFin));
                TryClose();
            }
            catch (Exception e)
            {
                await _dialogCoordinator.ShowMessageAsync(this, "Error", e.ToString());
            }
        }

        public void Cancelar()
        {
            TryClose();
        }
    }
}