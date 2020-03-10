using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Application.Empresas.Interfaces;
using Core.Application.Empresas.Models;
using MediatR;

namespace Core.Application.Empresas.Queries.BuscarEmpresasContabilidad
{
    public class BuscarEmpresasContabilidadQueryHandler : IRequestHandler<BuscarEmpresasContabilidadQuery, IEnumerable<EmpresaContpaqiDto>>
    {
        private readonly IEmpresaContabilidadRepository _empresaContabilidadRepository;

        public BuscarEmpresasContabilidadQueryHandler(IEmpresaContabilidadRepository empresaContabilidadRepository)
        {
            _empresaContabilidadRepository = empresaContabilidadRepository;
        }

        public Task<IEnumerable<EmpresaContpaqiDto>> Handle(BuscarEmpresasContabilidadQuery request, CancellationToken cancellationToken)
        {
            return _empresaContabilidadRepository.BuscarEmpresasAsync();
        }
    }
}