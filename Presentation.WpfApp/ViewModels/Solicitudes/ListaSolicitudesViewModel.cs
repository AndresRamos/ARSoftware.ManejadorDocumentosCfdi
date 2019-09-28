using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Data;
using Caliburn.Micro;
using Core.Application.Solicitudes.Commands.AutenticarSolicitud;
using Core.Application.Solicitudes.Models;
using Core.Application.Solicitudes.Queries.BuscarSolicitudesPorRangoFecha;
using MahApps.Metro.Controls.Dialogs;
using MediatR;

namespace Presentation.WpfApp.ViewModels.Solicitudes
{
    public sealed class ListaSolicitudesViewModel : Screen
    {
        private readonly IDialogCoordinator _dialogCoordinator;
        private readonly IMediator _mediator;
        private readonly IWindowManager _windowManager;
        private DateTime _fechaFin = DateTime.Today;
        private DateTime _fechaInicio = DateTime.Today;
        private SolicitudDto _solicitudSeleccionada;

        public ListaSolicitudesViewModel(IMediator mediator, IWindowManager windowManager, IDialogCoordinator dialogCoordinator)
        {
            _mediator = mediator;
            _windowManager = windowManager;
            _dialogCoordinator = dialogCoordinator;
            DisplayName = "Lista De Solicitudes";
            SolicitudesView = CollectionViewSource.GetDefaultView(Solicitudes);
        }

        public string Filtro { get; set; }

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
                RaiseGuards();
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
                RaiseGuards();
            }
        }

        public BindableCollection<SolicitudDto> Solicitudes { get; set; } = new BindableCollection<SolicitudDto>();

        public ICollectionView SolicitudesView { get; }

        public SolicitudDto SolicitudSeleccionada
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
                RaiseGuards();
            }
        }

        public bool CanProcesarSolicitudAsync => SolicitudSeleccionada != null;

        public async Task BuscarSolicitudesAsync()
        {
            Solicitudes.Clear();
            Solicitudes.AddRange(await _mediator.Send(new BuscarSolicitudesPorRangoFechaQuery(FechaInicio, FechaFin)));
        }

        public async Task CrearNuevaSolicitudAsync()
        {
            try
            {
                var viewModel = IoC.Get<NuevaSolicitudViewModel>();
                _windowManager.ShowDialog(viewModel);
                await BuscarSolicitudesAsync();
            }
            catch (Exception e)
            {
                await _dialogCoordinator.ShowMessageAsync(this, "Error", e.ToString());
            }
        }

        public async Task ProcesarSolicitudAsync()
        {
            try
            {
                await _mediator.Send(new AutenticarSolicitudCommand(SolicitudSeleccionada.Id));
            }
            catch (Exception e)
            {
                await _dialogCoordinator.ShowMessageAsync(this, "Error", e.ToString());
            }
        }

        private void RaiseGuards()
        {
            NotifyOfPropertyChange(() => CanProcesarSolicitudAsync);
        }
    }
}