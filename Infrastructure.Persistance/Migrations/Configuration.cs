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
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(ManejadorDocumentosCfdiDbContext context)
        {
            var configuracionGeneral = context.ConfiguracionGeneral.FirstOrDefault();
            if (configuracionGeneral is null)
            {
                configuracionGeneral = new ConfiguracionGeneral();
                context.Entry(configuracionGeneral).State = EntityState.Added;
                context.SaveChanges();
            }
        }
    }
}