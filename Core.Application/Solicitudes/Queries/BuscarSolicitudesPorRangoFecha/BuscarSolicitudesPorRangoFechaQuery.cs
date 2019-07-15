using System;
using System.Collections.Generic;
using Core.Application.Solicitudes.Models;
using MediatR;

namespace Core.Application.Solicitudes.Queries.BuscarSolicitudesPorRangoFecha
{
    public class BuscarSolicitudesPorRangoFechaQuery : IRequest<IEnumerable<SolicitudDto>>
    {
        public BuscarSolicitudesPorRangoFechaQuery(DateTime fechaInicio, DateTime fechaFin)
        {
            FechaInicio = fechaInicio;
            FechaFin = fechaFin;
        }

        public DateTime FechaInicio { get; }
        public DateTime FechaFin { get; }
    }
}