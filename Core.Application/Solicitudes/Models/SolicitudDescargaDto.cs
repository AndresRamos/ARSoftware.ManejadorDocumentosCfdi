using System;

namespace Core.Application.Solicitudes.Models
{
    public class SolicitudDescargaDto
    {
        public SolicitudDescargaDto(int id, DateTime fechaCreacionUtc, string codEstatus, string mensaje, string paqueteId, string paquete, string error, string solicitud, string respuesta, CodigoEstatusSolicitudDto codigoEstatusSolicitudEnum)
        {
            Id = id;
            FechaCreacionUtc = fechaCreacionUtc;
            FechaCreacionLocal = fechaCreacionUtc.ToLocalTime();
            CodEstatus = codEstatus;
            Mensaje = mensaje;
            PaqueteId = paqueteId;
            Paquete = paquete;
            Error = error;
            Solicitud = solicitud;
            Respuesta = respuesta;
            CodigoEstatusSolicitudEnum = codigoEstatusSolicitudEnum;
        }

        public int Id { get; set; }
        public DateTime FechaCreacionUtc { get; set; }
        public DateTime FechaCreacionLocal { get; set; }
        public string CodEstatus { get; set; }
        public string Mensaje { get; set; }
        public string PaqueteId { get; set; }
        public string Paquete { get; set; }
        public string Error { get; set; }
        public string Solicitud { get; set; }
        public string Respuesta { get; set; }
        public CodigoEstatusSolicitudDto CodigoEstatusSolicitudEnum { get; set; }

    }
}