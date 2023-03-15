using ARSoftware.Contpaqi.Contabilidad.Sql.Contexts;
using ARSoftware.Contpaqi.Contabilidad.Sql.Factories;
using Core.Application.Empresas.Interfaces;
using Core.Application.Empresas.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Contpaqi.Contabilidad.Repositories;

public class EmpresaContabilidadRepository : IEmpresaContabilidadRepository
{
    private readonly ContpaqiContabilidadGeneralesDbContext _context;

    public EmpresaContabilidadRepository(ContpaqiContabilidadGeneralesDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<EmpresaContpaqiDto>> BuscarEmpresasAsync()
    {
        var empresasContabilidad = await _context.ListaEmpresas.Select(e => new { e.Nombre, e.AliasBDD }).ToListAsync();

        var empresasList = new List<EmpresaContpaqiDto>();

        foreach (var empresaContabilidad in empresasContabilidad)
        {
            string empresaConnectionString =
                ContpaqiContabilidadSqlConnectionStringFactory.CreateContpaqiContabilidadEmpresaConnectionString(
                    _context.Database.GetConnectionString(),
                    new DirectoryInfo(empresaContabilidad.AliasBDD).Name);

            DbContextOptions<ContpaqiContabilidadEmpresaDbContext> empresaOptions =
                new DbContextOptionsBuilder<ContpaqiContabilidadEmpresaDbContext>().UseSqlServer(empresaConnectionString).Options;

            using (var contabilidadEmpresaDbContext = new ContpaqiContabilidadEmpresaDbContext(empresaOptions))
            {
                if (!await contabilidadEmpresaDbContext.Database.CanConnectAsync())
                    continue;

                string guidAddEmpresaContabilidad = await contabilidadEmpresaDbContext.Parametros.Select(p => p.GuidDSL).FirstAsync();

                empresasList.Add(new EmpresaContpaqiDto(empresaContabilidad.Nombre,
                    empresaContabilidad.AliasBDD,
                    guidAddEmpresaContabilidad));
            }
        }

        return empresasList;
    }
}
