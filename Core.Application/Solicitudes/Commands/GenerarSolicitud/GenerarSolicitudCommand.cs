using MediatR;

namespace Core.Application.Solicitudes.Commands.GenerarSolicitud;

public sealed record GenerarSolicitudCommand(int SolicitudId) : IRequest;
