using MediatR;

namespace Core.Application.Solicitudes.Commands.ProcesarSolicitud;

public sealed record ProcesarSolicitudCommand(int SolicitudId, int UsuarioId) : IRequest;
