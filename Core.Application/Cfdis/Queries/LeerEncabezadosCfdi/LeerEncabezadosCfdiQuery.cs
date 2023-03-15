using Core.Application.Cfdis.Models;
using MediatR;

namespace Core.Application.Cfdis.Queries.LeerEncabezadosCfdi;

public sealed record LeerEncabezadosCfdiQuery(IEnumerable<string> ArchivosCfdi) : IRequest<IEnumerable<CfdiEncabezadoDto>>;
