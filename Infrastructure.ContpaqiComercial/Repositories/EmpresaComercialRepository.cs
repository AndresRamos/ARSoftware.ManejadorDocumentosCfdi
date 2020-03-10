using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Contpaqi.Sql.Comercial.Generales;
using Core.Application.Empresas.Interfaces;
using Core.Application.Empresas.Models;

namespace Infrastructure.ContpaqiComercial.Repositories
{
    public class EmpresaComercialRepository : IEmpresaComercialRepository
    {
        private readonly ComercialGeneralesDbContext _context;

        public EmpresaComercialRepository(ComercialGeneralesDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EmpresaContpaqiDto>> BuscarEmpresasAsync()
        {
            return (await _context.Empresas.Select(e => new {e.CNOMBREEMPRESA, e.CRUTADATOS}).ToListAsync())
                .Select(e => new EmpresaContpaqiDto(e.CNOMBREEMPRESA, new DirectoryInfo(e.CRUTADATOS).Name)).ToList();
        }
    }
}