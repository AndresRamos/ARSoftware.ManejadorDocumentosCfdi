using System.Data.SqlClient;
using Contpaqi.Sql.Contabilidad.Empresa;

namespace Infrastructure.Contpaqi.Contabilidad.Factories
{
    public static class ContabilidadEmpresaDbContextFactory
    {
        public static ContabilidadEmpresaDbContext Crear(string contpaqiConnectionString, string initialCatalog)
        {
            var builder = new SqlConnectionStringBuilder(contpaqiConnectionString) { InitialCatalog = initialCatalog };
            return new ContabilidadEmpresaDbContext(new SqlConnection(builder.ToString()), true);
        }
    }
}
