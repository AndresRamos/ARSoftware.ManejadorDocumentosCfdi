using System;
using System.Threading.Tasks;
using Caliburn.Micro;
using Core.Application.Solicitudes.Models;
using Core.Application.Solicitudes.Queries.BuscarSolicitudPorId;
using MahApps.Metro.Controls.Dialogs;
using MediatR;
using Presentation.WpfApp.ViewModels.Xmls;

namespace Presentation.WpfApp.ViewModels.Solicitudes
{
    public sealed class DetalleSolicitudViewModel : Conductor<Screen>.Collection.OneActive
    {
        private readonly IDialogCoordinator _dialogCoordinator;
        private readonly IMediator _mediator;
        private readonly IWindowManager _windowManager;
        private SolicitudDto _solicitud;

        public DetalleSolicitudViewModel(IMediator mediator, IWindowManager windowManager, IDialogCoordinator dialogCoordinator)
        {
            _mediator = mediator;
            _windowManager = windowManager;
            _dialogCoordinator = dialogCoordinator;
            DisplayName = "Detalle Solicitud";
        }

        public SolicitudDto Solicitud
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

        public async Task InicializarAsync(int solicitudId)
        {
            Solicitud = await _mediator.Send(new BuscarSolicitudPorIdQuery(solicitudId));

            var solicitudViewModel = IoC.Get<SolicitudViewModel>();
            solicitudViewModel.Inicializar(Solicitud);
            Items.Add(solicitudViewModel);

            var solicitudAutenticacionViewModel = IoC.Get<SolicitudAutenticacionViewModel>();
            solicitudAutenticacionViewModel.Inicializar(Solicitud.Id, Solicitud.SolicitudAutenticacion, Solicitud.SolicitudesAutenticacion);
            Items.Add(solicitudAutenticacionViewModel);

            var solicitudSolicitudViewModel = IoC.Get<SolicitudSolicitudViewModel>();
            solicitudSolicitudViewModel.Inicializar(Solicitud.Id, Solicitud.SolicitudSolicitud, Solicitud.SolicitudesSolicitud);
            Items.Add(solicitudSolicitudViewModel);

            var solicitudVerificacionViewModel = IoC.Get<SolicitudVerificacionViewModel>();
            solicitudVerificacionViewModel.Inicializar(Solicitud.Id, Solicitud.SolicitudVerificacion, Solicitud.SolicitudesVerificacion);
            Items.Add(solicitudVerificacionViewModel);

            var solicitudDescargaViewModel = IoC.Get<SolicitudDescargaViewModel>();
            solicitudDescargaViewModel.Inicializar(Solicitud.Id, Solicitud.SolicitudDescarga, Solicitud.SolicitudesDescarga);
            Items.Add(solicitudDescargaViewModel);

            var solicitudPaquetesViewModel = IoC.Get<SolicitudPaquetesViewModel>();
            solicitudPaquetesViewModel.Inicializar(Solicitud.Paquetes);
            Items.Add(solicitudPaquetesViewModel);
        }

        public async Task VerXmlSolicitudAutenticaacionAsync()
        {
            try
            {
                var viewModel = IoC.Get<XmlViewerViewModel>();
                viewModel.Inicializar(Solicitud.SolicitudAutenticacion.Solicitud);
                _windowManager.ShowDialog(viewModel);
            }
            catch (Exception e)
            {
                await _dialogCoordinator.ShowMessageAsync(this, "Error", e.ToString());
            }
        }
    }
}