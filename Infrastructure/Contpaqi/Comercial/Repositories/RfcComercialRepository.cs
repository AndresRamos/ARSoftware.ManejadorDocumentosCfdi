using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Contpaqi.Sql.Comercial.Empresa;
using Core.Application.Rfcs.Interfaces;
using Core.Application.Rfcs.Models;

namespace Infrastructure.Contpaqi.Comercial.Repositories
{
    public class RfcComercialRepository : IRfcComercialRepository
    {
        private readonly ComercialEmpresaDbContext _context;

        public RfcComercialRepository(ComercialEmpresaDbContext context)
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
}
