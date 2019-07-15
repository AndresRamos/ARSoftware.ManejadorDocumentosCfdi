using System;
using System.IO;
using System.Threading.Tasks;
using Caliburn.Micro;
using Core.Application.Cfdis.Queries.ObtenerCertificado;
using MediatR;

namespace Presentation.WpfApp.ViewModels.Solicitudes
{
    public class NuevaSolicitudViewModel : Screen
    {
        private readonly IMediator _mediator;

        public NuevaSolicitudViewModel(IMediator mediator)
        {
            _mediator = mediator;
        }

        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }

        public async Task CrearSolicitudAsync()
        {
            var certificado = await _mediator.Send(new ObtenerCertificadoQuery(
                File.ReadAllBytes(@"C:\Users\gerar\OneDrive - AR Software\ClientArchive\SDN\Cert_Sellos\FIEL_SDN010719366_20170825100503\FIEL_SDN010719366_20170825100503\sdn010719366.cer"),
                "SDMRAMOS1"));
        }

        public void Cancelar()
        {
            TryClose();
        }
    }
}