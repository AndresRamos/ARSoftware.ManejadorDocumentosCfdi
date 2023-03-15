using ARSoftware.Contpaqi.Comercial.Sql.Contexts;
using Core.Application.Rfcs.Interfaces;
using Core.Application.Rfcs.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Contpaqi.Comercial.Repositories;

public class RfcComercialRepository : IRfcComercialRepository
{
    private readonly ContpaqiComercialEmpresaDbContext _context;

    public RfcComercialRepository(ContpaqiComercialEmpresaDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<RfcDto>> BuscarRfcsAsync()
    {
        return (await _context.admClientes.Select(c => new { c.CCODIGOCLIENTE, c.CRFC, c.CRAZONSOCIAL }).ToListAsync())
            .Select(c => new RfcDto(c.CCODIGOCLIENTE, c.CRFC, c.CRAZONSOCIAL))
            .ToList();
    }
}
