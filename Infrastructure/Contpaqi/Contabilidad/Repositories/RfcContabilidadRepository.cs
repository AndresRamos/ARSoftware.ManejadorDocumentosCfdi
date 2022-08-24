using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ARSoftware.Contpaqi.Contabilidad.Sql.Contexts;
using Core.Application.Rfcs.Interfaces;
using Core.Application.Rfcs.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Contpaqi.Contabilidad.Repositories;

public class RfcContabilidadRepository : IRfcContabilidadRepository
{
    private readonly ContpaqiContabilidadEmpresaDbContext _context;

    public RfcContabilidadRepository(ContpaqiContabilidadEmpresaDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<RfcDto>> BuscarRfcsAsync()
    {
        return (await _context.Personas.Select(p => new { p.Codigo, p.RFC, p.Nombre }).ToListAsync())
            .Select(p => new RfcDto(p.Codigo, p.RFC, p.Nombre))
            .ToList();
    }
}
