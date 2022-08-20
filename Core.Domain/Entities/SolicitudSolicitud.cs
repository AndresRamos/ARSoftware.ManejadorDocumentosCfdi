using System;

// ReSharper disable UnusedMember.Local
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local
// ReSharper disable UnusedAutoPropertyAccessor.Local

namespace Core.Domain.Entities
{
    public class SolicitudSolicitud : SolicitudWebBase
    {
        private SolicitudSolicitud()
        {
        }

        private SolicitudSolicitud(string solicitud,
                                   string respuesta,
                                   DateTime fechaInicio,
                                   DateTime fechaFin,
                                   string rfcEmisor,
                                   string rfcReceptor,
                                   string rfcSolicitante,
                                   string tipoSolicitud,
                                   string codEstatus,
                                   string mensaje,
                                   string idSolicitud,
                                   string error) : base(solicitud, respuesta)
        {
            FechaCreacionUtc = DateTime.UtcNow;
            FechaInicio = fechaInicio;
            FechaFin = fechaFin;
            RfcEmisor = rfcEmisor;
            RfcReceptor = rfcReceptor;
            RfcSolicitante = rfcSolicitante;
            TipoSolicitud = tipoSolicitud;
            IdSolicitud = idSolicitud;
            CodEstatus = codEstatus;
            Mensaje = mensaje;
            Error = error;
        }

        public DateTime FechaCreacionUtc { get; private set; }
        public DateTime FechaInicio { get; private set; }
        public DateTime FechaFin { get; private set; }
        public string RfcEmisor { get; private set; }
        public string RfcReceptor { get; private set; }
        public string RfcSolicitante { get; private set; }
        public string TipoSolicitud { get; private set; }
        public string CodEstatus { get; private set; }
        public string Mensaje { get; private set; }
        public string IdSolicitud { get; private set; }
        public string Error { get; private set; }
        public bool IsValid => !string.IsNullOrEmpty(IdSolicitud);

        public static SolicitudSolicitud CreateInstance(string solicitud,
                                                        string respuesta,
                                                        DateTime fechaInicio,
                                                        DateTime fechaFin,
                                                        string rfcEmisor,
                                                        string rfcReceptor,
                                                        string rfcSolicitante,
                                                        string tipoSolicitud,
                                                        string codEstatus,
                                                        string mensaje,
                                                        string idSolicitud,
                                                        string error)
        {
            return new SolicitudSolicitud(solicitud,
                respuesta,
                fechaInicio,
                fechaFin,
                rfcEmisor,
                rfcReceptor,
                rfcSolicitante,
                tipoSolicitud,
                codEstatus,
                mensaje,
                idSolicitud,
                error);
        }
    }
}
