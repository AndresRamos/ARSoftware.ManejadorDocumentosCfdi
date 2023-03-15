// ReSharper disable UnusedAutoPropertyAccessor.Local

namespace Core.Domain.Entities;

public class Paquete
{
    private Paquete()
    {
    }

    public int Id { get; private set; }
    public string IdSat { get; private set; }
    public byte[] Contenido { get; private set; }

    public static Paquete Crear(string idSat, byte[] contenido)
    {
        return new Paquete { IdSat = idSat, Contenido = contenido };
    }
}
