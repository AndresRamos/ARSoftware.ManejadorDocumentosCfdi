﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Application.Empresas.Interfaces;
using Core.Application.Empresas.Models;
using MediatR;

namespace Core.Application.Empresas.Queries.BuscarEmpresasComercial
{
    public class BuscarEmpresasComercialQueryHandler : IRequestHandler<BuscarEmpresasComercialQuery, IEnumerable<EmpresaContpaqiDto>>
    {
        private readonly IEmpresaComercialRepository _empresaComercialRepository;

        public BuscarEmpresasComercialQueryHandler(IEmpresaComercialRepository empresaComercialRepository)
        {
            _empresaComercialRepository = empresaComercialRepository;
        }

        public Task<IEnumerable<EmpresaContpaqiDto>> Handle(BuscarEmpresasComercialQuery request, CancellationToken cancellationToken)
        {
            return _empresaComercialRepository.BuscarEmpresasAsync();
        }
    }
}
