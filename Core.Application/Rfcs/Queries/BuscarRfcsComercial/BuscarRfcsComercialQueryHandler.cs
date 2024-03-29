﻿using Core.Application.Rfcs.Interfaces;
using Core.Application.Rfcs.Models;
using MediatR;

namespace Core.Application.Rfcs.Queries.BuscarRfcsComercial;

public sealed class BuscarRfcsComercialQueryHandler : IRequestHandler<BuscarRfcsComercialQuery, IEnumerable<RfcDto>>
{
    private readonly IRfcComercialRepository _rfcComercialRepository;

    public BuscarRfcsComercialQueryHandler(IRfcComercialRepository rfcComercialRepository)
    {
        _rfcComercialRepository = rfcComercialRepository;
    }

    public Task<IEnumerable<RfcDto>> Handle(BuscarRfcsComercialQuery request, CancellationToken cancellationToken)
    {
        return _rfcComercialRepository.BuscarRfcsAsync();
    }
}
