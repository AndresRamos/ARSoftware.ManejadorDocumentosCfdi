using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using Core.Domain.Entities;

namespace Infrastructure.Persistance.Migrations
{
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
}