using Core.Application.Solicitudes.Models;
using MediatR;

namespace Core.Application.Solicitudes.Queries.BuscarSolicitudPorId
{
    public class BuscarSolicitudPorIdQuery : IRequest<SolicitudDto>
    {
        public BuscarSolicitudPorIdQuery(int solicitudId)
        {
            SolicitudId = solicitudId;
        }

        public int SolicitudId { get; }
    }
}
