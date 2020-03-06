namespace Core.Application.TiposSolicitud.Models
{
    public class TipoSolicitudDto
    {
        public TipoSolicitudDto(string name, int id)
        {
            Name = name;
            Id = id;
        }

        public int Id { get; set; }
        public string Name { get; set; }
    }
}