namespace Infrastructure.Persistance.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ConfiguracionGeneral",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CertificadoSat_Certificado = c.Binary(),
                        CertificadoSat_Contrasena = c.String(),
                        CertificadoSat_Rfc = c.String(),
                        RutaDirectorioDescargas = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Paquete",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IdSat = c.String(),
                        Contenido = c.Binary(),
                        Solicitud_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Solicitud", t => t.Solicitud_Id)
                .Index(t => t.Solicitud_Id);
            
            CreateTable(
                "dbo.Solicitud",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FechaCreacionUtc = c.DateTime(nullable: false),
                        FechaInicio = c.DateTime(nullable: false),
                        FechaFin = c.DateTime(nullable: false),
                        RfcEmisor = c.String(),
                        RfcReceptor = c.String(),
                        RfcSolicitante = c.String(),
                        TipoSolicitud = c.String(),
                        SolicitudAutenticacionId = c.Int(),
                        SolicitudSolicitudId = c.Int(),
                        SolicitudVerificacionId = c.Int(),
                        SolicitudDescargaId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SolicitudWebBase", t => t.SolicitudAutenticacionId)
                .ForeignKey("dbo.SolicitudWebBase", t => t.SolicitudDescargaId)
                .ForeignKey("dbo.SolicitudWebBase", t => t.SolicitudSolicitudId)
                .ForeignKey("dbo.SolicitudWebBase", t => t.SolicitudVerificacionId)
                .Index(t => t.SolicitudAutenticacionId)
                .Index(t => t.SolicitudSolicitudId)
                .Index(t => t.SolicitudVerificacionId)
                .Index(t => t.SolicitudDescargaId);
            
            CreateTable(
                "dbo.SolicitudWebBase",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Solicitud = c.String(),
                        Respuesta = c.String(),
                        FechaCreacionUtc = c.DateTime(),
                        FechaTokenCreacionUtc = c.DateTime(),
                        FechaTokenExpiracionUtc = c.DateTime(),
                        Token = c.String(),
                        Autorizacion = c.String(),
                        FaultCode = c.String(),
                        FaultString = c.String(),
                        Error = c.String(),
                        FechaCreacionUtc1 = c.DateTime(),
                        CodEstatus = c.String(),
                        Mensaje = c.String(),
                        PaqueteId = c.String(),
                        Paquete = c.String(),
                        Error1 = c.String(),
                        FechaCreacionUtc2 = c.DateTime(),
                        FechaInicio = c.DateTime(),
                        FechaFin = c.DateTime(),
                        RfcEmisor = c.String(),
                        RfcReceptor = c.String(),
                        RfcSolicitante = c.String(),
                        TipoSolicitud = c.String(),
                        CodEstatus1 = c.String(),
                        Mensaje1 = c.String(),
                        IdSolicitud = c.String(),
                        Error2 = c.String(),
                        FechaCreacionUtc3 = c.DateTime(),
                        CodEstatus2 = c.String(),
                        Mensaje2 = c.String(),
                        CodigoEstadoSolicitud = c.String(),
                        EstadoSolicitud = c.String(),
                        NumeroCfdis = c.String(),
                        Error3 = c.String(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                        Solicitud_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Solicitud", t => t.Solicitud_Id)
                .Index(t => t.Solicitud_Id);
            
            CreateTable(
                "dbo.PaqueteId",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IdPaquete = c.String(),
                        IsDescargado = c.Boolean(nullable: false),
                        SolicitudVerificacion_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SolicitudWebBase", t => t.SolicitudVerificacion_Id)
                .Index(t => t.SolicitudVerificacion_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Solicitud", "SolicitudVerificacionId", "dbo.SolicitudWebBase");
            DropForeignKey("dbo.Solicitud", "SolicitudSolicitudId", "dbo.SolicitudWebBase");
            DropForeignKey("dbo.SolicitudWebBase", "Solicitud_Id", "dbo.Solicitud");
            DropForeignKey("dbo.PaqueteId", "SolicitudVerificacion_Id", "dbo.SolicitudWebBase");
            DropForeignKey("dbo.Solicitud", "SolicitudDescargaId", "dbo.SolicitudWebBase");
            DropForeignKey("dbo.Solicitud", "SolicitudAutenticacionId", "dbo.SolicitudWebBase");
            DropForeignKey("dbo.Paquete", "Solicitud_Id", "dbo.Solicitud");
            DropIndex("dbo.PaqueteId", new[] { "SolicitudVerificacion_Id" });
            DropIndex("dbo.SolicitudWebBase", new[] { "Solicitud_Id" });
            DropIndex("dbo.Solicitud", new[] { "SolicitudDescargaId" });
            DropIndex("dbo.Solicitud", new[] { "SolicitudVerificacionId" });
            DropIndex("dbo.Solicitud", new[] { "SolicitudSolicitudId" });
            DropIndex("dbo.Solicitud", new[] { "SolicitudAutenticacionId" });
            DropIndex("dbo.Paquete", new[] { "Solicitud_Id" });
            DropTable("dbo.PaqueteId");
            DropTable("dbo.SolicitudWebBase");
            DropTable("dbo.Solicitud");
            DropTable("dbo.Paquete");
            DropTable("dbo.ConfiguracionGeneral");
        }
    }
}
