using MediatR;

namespace Core.Application.Solicitudes.Commands.VerificarSolicitud;

public sealed record VerificarSolicitudCommand(int SolicitudId) : IRequest;
