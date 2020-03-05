using System;

namespace Core.Domain.Entities
{
    public class SolicitudDescarga : SolicitudWebBase
    {
        private SolicitudDescarga()
        {
        }

        private SolicitudDescarga(string solicitud, string respuesta, string codEstatus, string mensaje, string paqueteId, string paquete, string error) : base(solicitud, respuesta)
        {
            FechaCreacionUtc = DateTime.UtcNow;
            CodEstatus = codEstatus;
            Mensaje = mensaje;
            PaqueteId = paqueteId;
            Paquete = paquete;
            Error = error;
        }

        public DateTime FechaCreacionUtc { get; private set; }
        public string CodEstatus { get; private set; }
        public string Mensaje { get; private set; }
        public string PaqueteId { get; private set; }
        public string Paquete { get; private set; }
        public string Error { get; private set; }

        public static SolicitudDescarga CreateInstance(string solicitud, string respuesta, string codEstatus, string mensaje, string paqueteId, string paquete, string error)
        {
            return new SolicitudDescarga(solicitud, respuesta, codEstatus, mensaje, paqueteId, paquete, error);
        }
    }
}