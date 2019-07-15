using MediatR;

namespace Core.Application.Solicitudes.Commands.DescargarSolicitud
{
    public class DescargarSolicitudCommand : IRequest
    {
        public DescargarSolicitudCommand(int solicitudId)
        {
            SolicitudId = solicitudId;
        }

        public int SolicitudId { get; }
    }
}