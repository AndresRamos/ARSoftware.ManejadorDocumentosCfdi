using MediatR;

namespace Core.Application.Solicitudes.Commands.AutenticarSolicitud;

public sealed record AutenticarSolicitudCommand(int SolicitudId) : IRequest;
