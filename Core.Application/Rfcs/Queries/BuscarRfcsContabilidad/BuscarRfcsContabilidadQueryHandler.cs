using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Application.Rfcs.Interfaces;
using Core.Application.Rfcs.Models;
using MediatR;

namespace Core.Application.Rfcs.Queries.BuscarRfcsContabilidad
{
    public class BuscarRfcsContabilidadQueryHandler : IRequestHandler<BuscarRfcsContabilidadQuery, IEnumerable<RfcDto>>
    {
        private readonly IRfcContabilidadRepository _rfcContabilidadRepository;

        public BuscarRfcsContabilidadQueryHandler(IRfcContabilidadRepository rfcContabilidadRepository)
        {
            _rfcContabilidadRepository = rfcContabilidadRepository;
        }

        public Task<IEnumerable<RfcDto>> Handle(BuscarRfcsContabilidadQuery request, CancellationToken cancellationToken)
        {
            return _rfcContabilidadRepository.BuscarRfcsAsync();
        }
    }
}
