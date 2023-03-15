namespace Core.Application.Solicitudes.Models;

public sealed class EstadoSolicitudDto
{
    public EstadoSolicitudDto(int id, string estado)
    {
        Id = id;
        Estado = estado;
    }

    public int Id { get; set; }
    public string Estado { get; set; }

    public override string ToString()
    {
        return $"{Id} | {Estado}";
    }
}
