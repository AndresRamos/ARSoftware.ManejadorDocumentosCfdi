using System;
using MediatR;

namespace Core.Application.Solicitudes.Commands.CrearSolicitud
{
    public class CrearSolicitudCommand : IRequest<int>
    {
        public CrearSolicitudCommand(DateTime fechaInicio, DateTime fechaFin)
        {
            FechaInicio = fechaInicio;
            FechaFin = fechaFin;
        }

        public DateTime FechaInicio { get; }
        public DateTime FechaFin { get; }
    }
}