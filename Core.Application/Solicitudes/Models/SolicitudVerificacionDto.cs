using System;
using System.Collections.Generic;
using Core.Application.Paquetes.Models;

namespace Core.Application.Solicitudes.Models
{
    public class SolicitudVerificacionDto
    {
        public SolicitudVerificacionDto(int id, DateTime fechaCreacionUtc, string codEstatus, string mensaje, string codigoEstadoSolicitud, string estadoSolicitud, string numeroCfdis, IEnumerable<PaqueteIdDto> paqueteIds, string error, string solicitud, string respuesta, CodigoEstatusSolicitudDto codigoEstatusSolicitudEnum, EstadoSolicitudDto estadoSolicitudEnum, CodigoEstadoSolicitudDto codigoEstadoSolicitdEnum)
        {
            Id = id;
            FechaCreacionUtc = fechaCreacionUtc;
            FechaCreacionLocal = fechaCreacionUtc.ToLocalTime();
            CodEstatus = codEstatus;
            Mensaje = mensaje;
            CodigoEstadoSolicitud = codigoEstadoSolicitud;
            EstadoSolicitud = estadoSolicitud;
            NumeroCfdis = numeroCfdis;
            PaqueteIds = new List<PaqueteIdDto>(paqueteIds);
            Error = error;
            Solicitud = solicitud;
            Respuesta = respuesta;
            CodigoEstatusSolicitudEnum = codigoEstatusSolicitudEnum;
            EstadoSolicitudEnum = estadoSolicitudEnum;
            CodigoEstadoSolicitdEnum = codigoEstadoSolicitdEnum;
        }

        public int Id { get; set; }
        public DateTime FechaCreacionUtc { get; set; }
        public DateTime FechaCreacionLocal { get; set; }
        public string CodEstatus { get; set; }
        public string Mensaje { get; set; }
        public string CodigoEstadoSolicitud { get; set; }
        public string EstadoSolicitud { get; set; }
        public string NumeroCfdis { get; set; }
        public List<PaqueteIdDto> PaqueteIds { get; set; }
        public string Error { get; set; }
        public string Solicitud { get; set; }
        public string Respuesta { get; set; }
        public CodigoEstatusSolicitudDto CodigoEstatusSolicitudEnum { get; set; }
        public EstadoSolicitudDto EstadoSolicitudEnum { get; set; }
        public CodigoEstadoSolicitudDto CodigoEstadoSolicitdEnum { get; set; }
    }
}