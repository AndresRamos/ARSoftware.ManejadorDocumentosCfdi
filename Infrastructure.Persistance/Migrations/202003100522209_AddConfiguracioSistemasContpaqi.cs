namespace Infrastructure.Persistance.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddConfiguracioSistemasContpaqi : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ConfiguracionGeneral", "ConfiguracionContpaqiComercial_ContpaqiSqlConnectionString", c => c.String());
            AddColumn("dbo.ConfiguracionGeneral", "ConfiguracionContpaqiComercial_Empresa_Nombre", c => c.String());
            AddColumn("dbo.ConfiguracionGeneral", "ConfiguracionContpaqiComercial_Empresa_BaseDatos", c => c.String());
            AddColumn("dbo.ConfiguracionGeneral", "ConfiguracionContpaqiContabilidad_ContpaqiSqlConnectionString", c => c.String());
            AddColumn("dbo.ConfiguracionGeneral", "ConfiguracionContpaqiContabilidad_Empresa_Nombre", c => c.String());
            AddColumn("dbo.ConfiguracionGeneral", "ConfiguracionContpaqiContabilidad_Empresa_BaseDatos", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ConfiguracionGeneral", "ConfiguracionContpaqiContabilidad_Empresa_BaseDatos");
            DropColumn("dbo.ConfiguracionGeneral", "ConfiguracionContpaqiContabilidad_Empresa_Nombre");
            DropColumn("dbo.ConfiguracionGeneral", "ConfiguracionContpaqiContabilidad_ContpaqiSqlConnectionString");
            DropColumn("dbo.ConfiguracionGeneral", "ConfiguracionContpaqiComercial_Empresa_BaseDatos");
            DropColumn("dbo.ConfiguracionGeneral", "ConfiguracionContpaqiComercial_Empresa_Nombre");
            DropColumn("dbo.ConfiguracionGeneral", "ConfiguracionContpaqiComercial_ContpaqiSqlConnectionString");
        }
    }
}
