using System.Data.SqlClient;
using Contpaqi.Sql.ADD.DocumentMetadata;

namespace Infrastructure.Contpaqi.ADD.Factories
{
    public static class AddDocumentMetadataDbContextFactory
    {
        public static AddDocumentMetadataDbContext Crear(string contpaqiAddConnectionString, string guidCompany)
        {
            var builder = new SqlConnectionStringBuilder(contpaqiAddConnectionString)
            {
                InitialCatalog = $"document_{guidCompany}_metadata"
            };
            return new AddDocumentMetadataDbContext(new SqlConnection(builder.ConnectionString), true);
        }
    }
}
