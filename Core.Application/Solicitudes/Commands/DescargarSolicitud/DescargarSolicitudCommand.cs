using MediatR;

namespace Core.Application.Solicitudes.Commands.DescargarSolicitud;

public sealed record DescargarSolicitudCommand(int SolicitudId) : IRequest;
