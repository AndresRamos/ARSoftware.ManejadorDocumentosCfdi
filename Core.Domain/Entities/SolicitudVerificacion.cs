using System;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable UnusedMember.Local
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace Core.Domain.Entities
{
    public class SolicitudVerificacion : SolicitudWebBase
    {
        private SolicitudVerificacion()
        {
            PaquetesIds = new HashSet<PaqueteId>();
        }

        private SolicitudVerificacion(string solicitud,
                                      string respuesta,
                                      string codEstatus,
                                      string mensaje,
                                      string codigoEstadoSolicitud,
                                      string estadoSolicitud,
                                      string numeroCfdis,
                                      IEnumerable<PaqueteId> idsPaquetes,
                                      string error) : base(solicitud, respuesta)
        {
            FechaCreacionUtc = DateTime.UtcNow;
            CodEstatus = codEstatus;
            Mensaje = mensaje;
            CodigoEstadoSolicitud = codigoEstadoSolicitud;
            EstadoSolicitud = estadoSolicitud;
            NumeroCfdis = numeroCfdis;
            Error = error;

            PaquetesIds = new HashSet<PaqueteId>();
            foreach (PaqueteId idsPaquete in idsPaquetes)
            {
                PaquetesIds.Add(idsPaquete);
            }
        }

        public DateTime FechaCreacionUtc { get; private set; }
        public string CodEstatus { get; private set; }
        public string Mensaje { get; private set; }
        public string CodigoEstadoSolicitud { get; private set; }
        public string EstadoSolicitud { get; private set; }
        public string NumeroCfdis { get; private set; }
        public string Error { get; private set; }
        public ICollection<PaqueteId> PaquetesIds { get; private set; }

        public bool IsValid => !string.IsNullOrEmpty(EstadoSolicitud) && EstadoSolicitud == "3";

        public bool HasPaquetesPendientesPorDescargar => PaquetesIds.Any(p => !p.IsDescargado);

        public static SolicitudVerificacion CreateInstance(string solicitud,
                                                           string respuesta,
                                                           string codEstatus,
                                                           string mensaje,
                                                           string codigoEstadoSolicitud,
                                                           string estadoSolicitud,
                                                           string numeroCfdis,
                                                           IEnumerable<PaqueteId> idsPaquetes,
                                                           string error)
        {
            return new SolicitudVerificacion(solicitud,
                respuesta,
                codEstatus,
                mensaje,
                codigoEstadoSolicitud,
                estadoSolicitud,
                numeroCfdis,
                idsPaquetes,
                error);
        }
    }
}
