using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Contpaqi.Sql.Contabilidad.Empresa;
using Core.Application.Rfcs.Interfaces;
using Core.Application.Rfcs.Models;

namespace Infrastructure.ContpaqiContabilidad.Repositories
{
    public class RfcContabilidadRepository : IRfcContabilidadRepository
    {
        private readonly ContabilidadEmpresaDbContext _context;

        public RfcContabilidadRepository(ContabilidadEmpresaDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RfcDto>> BuscarRfcsAsync()
        {
            return (await _context.Personas.Select(p => new {p.Codigo, p.RFC, p.Nombre}).ToListAsync())
                .Select(p => new RfcDto(p.Codigo, p.RFC, p.Nombre)).ToList();
        }
    }
}