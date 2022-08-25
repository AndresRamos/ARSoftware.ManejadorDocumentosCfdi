using MediatR;

namespace Core.Application.Solicitudes.Commands.VerificarSolicitud;

public class VerificarSolicitudCommand : IRequest
{
    public VerificarSolicitudCommand(int solicitudId)
    {
        SolicitudId = solicitudId;
    }

    public int SolicitudId { get; }
}
