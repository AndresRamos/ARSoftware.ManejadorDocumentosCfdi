using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Contpaqi.Sql.Contabilidad.Generales;
using Core.Application.Empresas.Interfaces;
using Core.Application.Empresas.Models;

namespace Infrastructure.ContpaqiContabilidad.Repositories
{
    public class EmpresaContabilidadRepository : IEmpresaContabilidadRepository
    {
        private readonly ContabilidadGeneralesDbContext _context;

        public EmpresaContabilidadRepository(ContabilidadGeneralesDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EmpresaContpaqiDto>> BuscarEmpresasAsync()
        {
            return (await _context.ListaEmpresas.Select(e => new {e.Nombre, e.AliasBDD}).ToListAsync())
                .Select(e => new EmpresaContpaqiDto(e.Nombre, e.AliasBDD)).ToList();
        }
    }
}