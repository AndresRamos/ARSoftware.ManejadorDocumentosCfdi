﻿using System.Data.Entity;
using Core.Application.Empresas.Models;
using Infrastructure.Persistance;
using MediatR;

namespace Core.Application.Empresas.Queries.BuscarEmpresas;

public sealed class BuscarEmpresasQueryHandler : IRequestHandler<BuscarEmpresasQuery, IEnumerable<EmpresaPerfilDto>>
{
    private readonly ManejadorDocumentosCfdiDbContext _context;

    public BuscarEmpresasQueryHandler(ManejadorDocumentosCfdiDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<EmpresaPerfilDto>> Handle(BuscarEmpresasQuery request, CancellationToken cancellationToken)
    {
        return await _context.Empresas.OrderBy(e => e.Nombre)
            .Select(e => new EmpresaPerfilDto { Id = e.Id, Nombre = e.Nombre })
            .ToListAsync(cancellationToken);
    }
}
