using MediatR;

namespace Core.Application.Solicitudes.Commands.ProcesarSolicitud;

public class ProcesarSolicitudCommand : IRequest
{
    public ProcesarSolicitudCommand(int solicitudId, int usuarioId)
    {
        SolicitudId = solicitudId;
        UsuarioId = usuarioId;
    }

    public int SolicitudId { get; }
    public int UsuarioId { get; }
}
