using Core.Application.Rfcs.Models;
using MediatR;

namespace Core.Application.Rfcs.Queries.BuscarRfcsComercial;

public sealed record BuscarRfcsComercialQuery : IRequest<IEnumerable<RfcDto>>;
