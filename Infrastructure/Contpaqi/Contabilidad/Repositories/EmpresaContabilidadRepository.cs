using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Contpaqi.Sql.Contabilidad.Empresa;
using Contpaqi.Sql.Contabilidad.Generales;
using Core.Application.Empresas.Interfaces;
using Core.Application.Empresas.Models;
using Infrastructure.Contpaqi.Contabilidad.Factories;

namespace Infrastructure.Contpaqi.Contabilidad.Repositories
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
            var empresasContabilidad = await _context.ListaEmpresas.Select(e => new { e.Nombre, e.AliasBDD }).ToListAsync();

            var empresasList = new List<EmpresaContpaqiDto>();

            foreach (var empresaContabilidad in empresasContabilidad)
            {
                using (ContabilidadEmpresaDbContext contabilidadEmpresaDbContext =
                       ContabilidadEmpresaDbContextFactory.Crear(_context.Database.Connection.ConnectionString,
                           empresaContabilidad.AliasBDD))
                {
                    if (!contabilidadEmpresaDbContext.Database.Exists())
                    {
                        continue;
                    }

                    string guidAddEmpresaContabilidad = await contabilidadEmpresaDbContext.Parametros.Select(p => p.GuidDSL).FirstAsync();

                    empresasList.Add(new EmpresaContpaqiDto(empresaContabilidad.Nombre,
                        empresaContabilidad.AliasBDD,
                        guidAddEmpresaContabilidad));
                }
            }

            return empresasList;
        }
    }
}
