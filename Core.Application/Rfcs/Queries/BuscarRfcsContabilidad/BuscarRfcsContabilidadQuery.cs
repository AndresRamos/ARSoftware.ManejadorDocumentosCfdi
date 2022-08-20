using System.Collections.Generic;
using Core.Application.Rfcs.Models;
using MediatR;

namespace Core.Application.Rfcs.Queries.BuscarRfcsContabilidad
{
    public class BuscarRfcsContabilidadQuery : IRequest<IEnumerable<RfcDto>>
    {
    }
}
