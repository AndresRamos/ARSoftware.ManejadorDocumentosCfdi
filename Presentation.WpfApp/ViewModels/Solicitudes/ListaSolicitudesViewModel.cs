using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Data;
using Caliburn.Micro;
using Core.Application.Solicitudes.Commands.ProcesarSolicitud;
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
        private string _filtro;
        private SolicitudDto _solicitudSeleccionada;

        public ListaSolicitudesViewModel(IMediator mediator, IWindowManager windowManager, IDialogCoordinator dialogCoordinator)
        {
            _mediator = mediator;
            _windowManager = windowManager;
            _dialogCoordinator = dialogCoordinator;
            DisplayName = "Lista De Solicitudes";
            SolicitudesView = CollectionViewSource.GetDefaultView(Solicitudes);
        }

        public string Filtro
        {
            get => _filtro;
            set
            {
                if (value == _filtro)
                {
                    return;
                }

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

        public bool CanBuscarSolicitudesAsync => FechaInicio <= FechaFin;

        public bool CanProcesarSolicitudAsync => SolicitudSeleccionada != null;

        public bool CanVerDetallesSolicitudSeleccionadaAsync => SolicitudSeleccionada != null;

        public async Task BuscarSolicitudesAsync()
        {
            var progressDialogController = await _dialogCoordinator.ShowProgressAsync(this, "Buscando Solicitudes", "Buscando solicitudes.");
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
                _windowManager.ShowDialog(viewModel);
                await BuscarSolicitudesAsync();
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

        public async Task ProcesarSolicitudAsync()
        {
            var progressDialogController = await _dialogCoordinator.ShowProgressAsync(this, "Procesando Solicitud", "Procesando solicitud.");
            progressDialogController.SetIndeterminate();
            await Task.Delay(1000);

            try
            {
                await _mediator.Send(new ProcesarSolicitudCommand(SolicitudSeleccionada.Id));
                await BuscarSolicitudesAsync();
                await _dialogCoordinator.ShowMessageAsync(this, "Solicitud Procesada", "La solicitud termino de procesarse exitosamente.");
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

        public async Task VerDetallesSolicitudSeleccionadaAsync()
        {
            try
            {
                var viewModel = IoC.Get<DetalleSolicitudViewModel>();
                await viewModel.InicializarAsync(SolicitudSeleccionada.Id);
                _windowManager.ShowDialog(viewModel);
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
            NotifyOfPropertyChange(() => CanProcesarSolicitudAsync);
            NotifyOfPropertyChange(() => CanVerDetallesSolicitudSeleccionadaAsync);
        }
    }
}