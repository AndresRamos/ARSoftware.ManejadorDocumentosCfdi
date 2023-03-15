namespace Core.Application.Paquetes.Models;

public sealed class PaqueteDto
{
    public PaqueteDto(int id, string idSat, byte[] contenido)
    {
        Id = id;
        IdSat = idSat;
        Contenido = contenido;
    }

    public int Id { get; set; }
    public string IdSat { get; set; }
    public byte[] Contenido { get; set; }
}
