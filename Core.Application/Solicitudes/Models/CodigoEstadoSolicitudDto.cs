namespace Core.Application.Solicitudes.Models
{
    public class CodigoEstadoSolicitudDto
    {
        public CodigoEstadoSolicitudDto(int id, string codigo, string mensaje, string observaciones)
        {
            Id = id;
            Codigo = codigo;
            Mensaje = mensaje;
            Observaciones = observaciones;
        }

        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Mensaje { get; set; }
        public string Observaciones { get; set; }

        public override string ToString()
        {
            return $"{Codigo} | {Mensaje} | {Observaciones}";
        }
    }
}
