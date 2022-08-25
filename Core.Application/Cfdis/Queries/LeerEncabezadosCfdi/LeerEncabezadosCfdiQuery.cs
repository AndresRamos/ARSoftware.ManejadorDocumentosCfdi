using System.Collections.Generic;
using MediatR;

namespace Core.Application.Cfdis.Queries.LeerEncabezadosCfdi;

public class LeerEncabezadosCfdiQuery : IRequest<IEnumerable<CfdiEncabezadoDto>>
{
    public LeerEncabezadosCfdiQuery(IEnumerable<string> archivosCfdi)
    {
        ArchivosCfdi = archivosCfdi;
    }

    public IEnumerable<string> ArchivosCfdi { get; }
}
