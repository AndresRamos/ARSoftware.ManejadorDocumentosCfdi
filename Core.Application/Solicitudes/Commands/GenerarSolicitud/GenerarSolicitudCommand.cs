using MediatR;

namespace Core.Application.Solicitudes.Commands.GenerarSolicitud
{
    public class GenerarSolicitudCommand : IRequest
    {
        public GenerarSolicitudCommand(int solicitudId)
        {
            SolicitudId = solicitudId;
        }

        public int SolicitudId { get; }
    }
}