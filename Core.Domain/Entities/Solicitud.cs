using System;

namespace Core.Domain.Entities
{
    public class Solicitud
    {
        public Solicitud()
        {
        }

        public int Id { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string Token { get; set; }
        public string Autorizacion { get; set; }
        public string SolicitudSatId { get; set; }
        public string PaqueteId { get; set; }

        public static Solicitud CreateNew(DateTime fechaInicio, DateTime fechaFin)
        {
            var solicitud = new Solicitud();
            solicitud.FechaCreacion = DateTime.Today;
            solicitud.FechaInicio = fechaInicio;
            solicitud.FechaFin = fechaFin;
            return solicitud;
        }
    }
}