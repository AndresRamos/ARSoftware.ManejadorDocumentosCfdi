using Core.Application.Rfcs.Models;
using MediatR;

namespace Core.Application.Rfcs.Queries.BuscarRfcsContabilidad;

public sealed record BuscarRfcsContabilidadQuery : IRequest<IEnumerable<RfcDto>>;
