using System;

namespace Core.Application.Solicitudes.Models
{
    public class SolicitudDto
    {
        public int Id { get; set; }
        public DateTime FechaCreacion { get; set; }

        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string Token { get; set; }
    }
}