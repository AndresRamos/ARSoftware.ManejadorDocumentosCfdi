using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ARSoftware.Contpaqi.Comercial.Sql.Contexts;
using ARSoftware.Contpaqi.Comercial.Sql.Factories;
using Core.Application.Empresas.Interfaces;
using Core.Application.Empresas.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Contpaqi.Comercial.Repositories;

public class EmpresaComercialRepository : IEmpresaComercialRepository
{
    private readonly ContpaqiComercialGeneralesDbContext _context;

    public EmpresaComercialRepository(ContpaqiComercialGeneralesDbContext context)
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
            string empresaConnectionString = ContpaqiComercialSqlConnectionStringFactory.CreateContpaqiComercialEmpresaConnectionString(
                _context.Database.GetConnectionString(),
                new DirectoryInfo(empresaComercial.CRUTADATOS).Name);

            DbContextOptions<ContpaqiComercialEmpresaDbContext> empresaOptions =
                new DbContextOptionsBuilder<ContpaqiComercialEmpresaDbContext>().UseSqlServer(empresaConnectionString).Options;

            using (var comercialEmpresaDbContext = new ContpaqiComercialEmpresaDbContext(empresaOptions))
            {
                if (!await comercialEmpresaDbContext.Database.CanConnectAsync())
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
