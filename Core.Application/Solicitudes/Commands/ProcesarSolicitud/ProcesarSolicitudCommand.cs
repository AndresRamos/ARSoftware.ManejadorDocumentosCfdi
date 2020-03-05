using MediatR;

namespace Core.Application.Solicitudes.Commands.ProcesarSolicitud
{
    public class ProcesarSolicitudCommand : IRequest
    {
        public ProcesarSolicitudCommand(int solicitudId)
        {
            SolicitudId = solicitudId;
        }

        public int SolicitudId { get; }
    }
}