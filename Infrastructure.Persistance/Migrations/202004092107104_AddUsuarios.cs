namespace Infrastructure.Persistance.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUsuarios : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Rol",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nombre = c.String(),
                        Descripcion = c.String(),
                        Permisos = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Usuario",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PrimerNombre = c.String(),
                        Apellido = c.String(),
                        Email = c.String(),
                        NombreUsuario = c.String(),
                        PasswordHash = c.String(),
                        PasswordSalt = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UsuarioRol",
                c => new
                    {
                        Usuario_Id = c.Int(nullable: false),
                        Rol_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Usuario_Id, t.Rol_Id })
                .ForeignKey("dbo.Usuario", t => t.Usuario_Id, cascadeDelete: true)
                .ForeignKey("dbo.Rol", t => t.Rol_Id, cascadeDelete: true)
                .Index(t => t.Usuario_Id)
                .Index(t => t.Rol_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UsuarioRol", "Rol_Id", "dbo.Rol");
            DropForeignKey("dbo.UsuarioRol", "Usuario_Id", "dbo.Usuario");
            DropIndex("dbo.UsuarioRol", new[] { "Rol_Id" });
            DropIndex("dbo.UsuarioRol", new[] { "Usuario_Id" });
            DropTable("dbo.UsuarioRol");
            DropTable("dbo.Usuario");
            DropTable("dbo.Rol");
        }
    }
}
