using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using Core.Domain.Entities;
using Core.Domain.ValueObjects;
using Infrastructure.Persistance.Migrations;

namespace Infrastructure.Persistance
{
    public class ManejadorDocumentosCfdiDbContext : DbContext
    {
        public ManejadorDocumentosCfdiDbContext() : base("ManejadorDocumentosCfdiDbContext")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ManejadorDocumentosCfdiDbContext, Configuration>());
        }

        public DbSet<ConfiguracionGeneral> ConfiguracionGeneral { get; set; }

        public DbSet<Solicitud> Solicitudes { get; set; }

        public DbSet<SolicitudWebBase> SolicitudesWeb { get; set; }

        public DbSet<Paquete> Paquetes { get; set; }

        public DbSet<Usuario> Usuarios { get; set; }

        public DbSet<Rol> Roles { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.ComplexType<CertificadoSat>();
            modelBuilder.ComplexType<ConfiguracionContpaqiComercial>();
            modelBuilder.ComplexType<ConfiguracionContpaqiContabilidad>();
            modelBuilder.ComplexType<EmpresaContpaqi>();
        }
    }
}