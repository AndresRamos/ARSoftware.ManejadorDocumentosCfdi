using Core.Application.Solicitudes.Models;
using MediatR;

namespace Core.Application.Solicitudes.Queries.BuscarSolicitudPorId;

public sealed record BuscarSolicitudPorIdQuery(int SolicitudId) : IRequest<SolicitudDto>;
