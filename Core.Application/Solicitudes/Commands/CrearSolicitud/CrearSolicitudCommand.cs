using System;
using MediatR;

namespace Core.Application.Solicitudes.Commands.CrearSolicitud
{
    public class CrearSolicitudCommand : IRequest<int>
    {
        public CrearSolicitudCommand(DateTime fechaInicio, DateTime fechaFin, string rfcEmisor, string rfcReceptor, string rfcSolicitante, string tipoSolicitud)
        {
            FechaInicio = fechaInicio;
            FechaFin = fechaFin;
            RfcEmisor = rfcEmisor;
            RfcReceptor = rfcReceptor;
            RfcSolicitante = rfcSolicitante;
            TipoSolicitud = tipoSolicitud;
        }

        public DateTime FechaInicio { get; }
        public DateTime FechaFin { get; }
        public string RfcEmisor { get; }
        public string RfcReceptor { get; }
        public string RfcSolicitante { get; }
        public string TipoSolicitud { get; }
    }
}