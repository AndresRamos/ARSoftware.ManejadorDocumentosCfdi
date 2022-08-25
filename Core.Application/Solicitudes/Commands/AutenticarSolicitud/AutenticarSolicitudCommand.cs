using MediatR;

namespace Core.Application.Solicitudes.Commands.AutenticarSolicitud;

public class AutenticarSolicitudCommand : IRequest
{
    public AutenticarSolicitudCommand(int solicitudId)
    {
        SolicitudId = solicitudId;
    }

    public int SolicitudId { get; }
}
