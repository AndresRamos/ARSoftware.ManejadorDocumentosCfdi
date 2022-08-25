using System.Data.Entity.Migrations;

namespace Infrastructure.Persistance.Migrations;

internal sealed class Configuration : DbMigrationsConfiguration<ManejadorDocumentosCfdiDbContext>
{
    public Configuration()
    {
        AutomaticMigrationsEnabled = true;
    }

    protected override void Seed(ManejadorDocumentosCfdiDbContext context)
    {
    }
}
