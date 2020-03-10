using System.Collections.Generic;
using Core.Application.Rfcs.Models;
using MediatR;

namespace Core.Application.Rfcs.Queries.BuscarRfcsComercial
{
    public class BuscarRfcsComercialQuery : IRequest<IEnumerable<RfcDto>>
    {
    }
}