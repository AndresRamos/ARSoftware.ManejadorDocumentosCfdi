using Core.Application.TiposSolicitud.Models;
using MediatR;

namespace Core.Application.TiposSolicitud.Queries.BuscarTiposSolicitud;

public sealed record BuscarTiposSolicitudQuery : IRequest<IEnumerable<TipoSolicitudDto>>;
