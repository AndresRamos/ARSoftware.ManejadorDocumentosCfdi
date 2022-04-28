using System;
using System.Collections.Generic;

namespace Core.Domain.Entities
{
    public class Solicitud
    {
        public Solicitud()
        {
            SolicitudesWeb = new HashSet<SolicitudWebBase>();
            Paquetes = new HashSet<Paquete>();
        }

        public int Id { get; set; }
        public DateTime FechaCreacionUtc { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string RfcEmisor { get; set; }
        public string RfcReceptor { get; set; }
        public string RfcSolicitante { get; set; }
        public string TipoSolicitud { get; set; }

        public IEnumerable<string> Receptores => RfcReceptor.Split('|');

        public int? SolicitudAutenticacionId { get; set; }
        public SolicitudAutenticacion SolicitudAutenticacion { get; set; }

        public int? SolicitudSolicitudId { get; set; }
        public SolicitudSolicitud SolicitudSolicitud { get; set; }

        public int? SolicitudVerificacionId { get; set; }
        public SolicitudVerificacion SolicitudVerificacion { get; set; }

        public int? SolicitudDescargaId { get; set; }
        public SolicitudDescarga SolicitudDescarga { get; set; }

        public ICollection<SolicitudWebBase> SolicitudesWeb { get; set; }

        public ICollection<Paquete> Paquetes { get; set; }

        public int EmpresaId { get; set; }
        public Empresa Empresa { get; set; }

        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }

        public static Solicitud CreateNew(int empresaId,int usuarioId, DateTime fechaInicio, DateTime fechaFin, string rfcEmisor, string rfcReceptor, string rfcSolicitante, string tipoSolicitud)
        {
            var solicitud = new Solicitud
            {
                EmpresaId = empresaId,
                UsuarioId = usuarioId,
                FechaCreacionUtc = DateTime.UtcNow,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin,
                RfcEmisor = rfcEmisor,
                RfcReceptor = rfcReceptor,
                RfcSolicitante = rfcSolicitante,
                TipoSolicitud = tipoSolicitud
            };
            return solicitud;
        }
    }
}