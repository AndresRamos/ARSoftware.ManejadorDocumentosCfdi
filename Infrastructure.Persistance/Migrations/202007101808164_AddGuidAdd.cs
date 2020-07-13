namespace Infrastructure.Persistance.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddGuidAdd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ConfiguracionGeneral", "ConfiguracionContpaqiComercial_Empresa_GuidAdd", c => c.String());
            AddColumn("dbo.ConfiguracionGeneral", "ConfiguracionContpaqiContabilidad_Empresa_GuidAdd", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ConfiguracionGeneral", "ConfiguracionContpaqiContabilidad_Empresa_GuidAdd");
            DropColumn("dbo.ConfiguracionGeneral", "ConfiguracionContpaqiComercial_Empresa_GuidAdd");
        }
    }
}
