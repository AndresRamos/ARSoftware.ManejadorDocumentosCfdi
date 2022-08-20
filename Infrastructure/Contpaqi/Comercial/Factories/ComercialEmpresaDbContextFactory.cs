using System;
using System.Data.SqlClient;
using Contpaqi.Sql.Comercial.Empresa;

namespace Infrastructure.Contpaqi.Comercial.Factories
{
    public static class ComercialEmpresaDbContextFactory
    {
        public static ComercialEmpresaDbContext Crear(string contpaqiConnectionString, string initialCatalog)
        {
            if (contpaqiConnectionString == null)
            {
                throw new ArgumentNullException(nameof(contpaqiConnectionString));
            }

            if (initialCatalog == null)
            {
                throw new ArgumentNullException(nameof(initialCatalog));
            }

            var builder = new SqlConnectionStringBuilder(contpaqiConnectionString) { InitialCatalog = initialCatalog };
            return new ComercialEmpresaDbContext(new SqlConnection(builder.ToString()), true);
        }
    }
}
