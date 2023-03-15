namespace Core.Application.Paquetes.Models;

public sealed class PaqueteIdDto
{
    public PaqueteIdDto(int id, string idPaquete, bool isDescargado)
    {
        Id = id;
        IdPaquete = idPaquete;
        IsDescargado = isDescargado;
    }

    public int Id { get; set; }
    public string IdPaquete { get; set; }
    public bool IsDescargado { get; set; }
}
