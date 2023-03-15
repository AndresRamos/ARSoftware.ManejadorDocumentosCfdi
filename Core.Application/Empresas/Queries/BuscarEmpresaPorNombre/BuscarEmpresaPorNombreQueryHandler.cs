using System.Data.Entity;
using Core.Application.Empresas.Models;
using Core.Domain.Entities;
using Infrastructure.Persistance;
using MediatR;

namespace Core.Application.Empresas.Queries.BuscarEmpresaPorNombre;

public sealed class BuscarEmpresaPorNombreQueryHandler : IRequestHandler<BuscarEmpresaPorNombreQuery, EmpresaPerfilDto>
{
    private readonly ManejadorDocumentosCfdiDbContext _context;

    public BuscarEmpresaPorNombreQueryHandler(ManejadorDocumentosCfdiDbContext context)
    {
        _context = context;
    }

    public async Task<EmpresaPerfilDto> Handle(BuscarEmpresaPorNombreQuery request, CancellationToken cancellationToken)
    {
        Empresa empresa = await _context.Empresas.SingleOrDefaultAsync(e => e.Nombre == request.EmpresaNombre, cancellationToken);

        if (empresa is null)
            return null;

        return new EmpresaPerfilDto { Id = empresa.Id, Nombre = empresa.Nombre };
    }
}
