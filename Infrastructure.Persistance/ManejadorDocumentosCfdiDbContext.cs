using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Net.Configuration;
using Core.Domain.Entities;
using Core.Domain.ValueObjects;

namespace Infrastructure.Persistance
{
    public class ManejadorDocumentosCfdiDbContext : DbContext
    {
        public ManejadorDocumentosCfdiDbContext() : base("ManejadorDocumentosCfdiDbContext")
        {
        }

        public DbSet<ConfiguracionGeneral> ConfiguracionGeneral { get; set; }
        public DbSet<Solicitud> Solicitudes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.ComplexType<CertificadoSat>();
        }
    }
}