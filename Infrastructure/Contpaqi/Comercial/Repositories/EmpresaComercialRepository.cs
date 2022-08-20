using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Contpaqi.Sql.Comercial.Empresa;
using Contpaqi.Sql.Comercial.Generales;
using Core.Application.Empresas.Interfaces;
using Core.Application.Empresas.Models;
using Infrastructure.Contpaqi.Comercial.Factories;

namespace Infrastructure.Contpaqi.Comercial.Repositories
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
            var empresasComercial = await _context.Empresas.Where(e => e.CIDEMPRESA != 1)
                .Select(e => new { e.CNOMBREEMPRESA, e.CRUTADATOS })
                .ToListAsync();

            var empresasList = new List<EmpresaContpaqiDto>();

            foreach (var empresaComercial in empresasComercial)
            {
                using (ComercialEmpresaDbContext comercialEmpresaDbContext =
                       ComercialEmpresaDbContextFactory.Crear(_context.Database.Connection.ConnectionString,
                           new DirectoryInfo(empresaComercial.CRUTADATOS).Name))
                {
                    if (!comercialEmpresaDbContext.Database.Exists())
                    {
                        continue;
                    }

                    string guidAddEmpresaComercial = await comercialEmpresaDbContext.admParametros.Select(p => p.CGUIDDSL).FirstAsync();

                    empresasList.Add(new EmpresaContpaqiDto(empresaComercial.CNOMBREEMPRESA,
                        new DirectoryInfo(empresaComercial.CRUTADATOS).Name,
                        guidAddEmpresaComercial));
                }
            }

            return empresasList;
        }
    }
}
