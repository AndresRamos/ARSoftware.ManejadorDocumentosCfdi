using System.Data.SqlClient;
using Contpaqi.Sql.Contabilidad.Generales;

namespace Infrastructure.ContpaqiContabilidad.Factories
{
    public static class ContabilidadGeneralesDbContextFactory
    {
        public static ContabilidadGeneralesDbContext Crear(string contpaqiConnectionString, string initialCatalog = "GeneralesSQL")
        {
            var builder = new SqlConnectionStringBuilder(contpaqiConnectionString) {InitialCatalog = initialCatalog};
            return new ContabilidadGeneralesDbContext(new SqlConnection(builder.ToString()), true);
        }
    }
}