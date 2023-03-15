using Core.Application.Solicitudes.Models;
using MediatR;

namespace Core.Application.Solicitudes.Queries.BuscarSolicitudesPorRangoFecha;

public sealed record BuscarSolicitudesPorRangoFechaQuery(DateTime FechaInicio, DateTime FechaFin) : IRequest<IEnumerable<SolicitudDto>>;
