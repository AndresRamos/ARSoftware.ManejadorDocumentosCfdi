using System;
using System.Data.SqlClient;
using Contpaqi.Sql.Comercial.Generales;

namespace Infrastructure.Contpaqi.Comercial.Factories
{
    public static class ComercialGeneralesDbContextFactory
    {
        public static ComercialGeneralesDbContext Crear(string contpaqiConnectionString, string initialCatalog = "CompacWAdmin")
        {
            if (contpaqiConnectionString == null)
            {
                throw new ArgumentNullException(nameof(contpaqiConnectionString));
            }

            var builder = new SqlConnectionStringBuilder(contpaqiConnectionString) { InitialCatalog = initialCatalog };
            return new ComercialGeneralesDbContext(new SqlConnection(builder.ToString()), true);
        }
    }
}
