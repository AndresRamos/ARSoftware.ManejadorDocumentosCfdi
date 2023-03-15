// ReSharper disable UnusedAutoPropertyAccessor.Local

namespace Core.Domain.Entities;

public class Empresa
{
    private Empresa()
    {
    }

    public int Id { get; private set; }
    public string Nombre { get; private set; }
    public ConfiguracionGeneral ConfiguracionGeneral { get; private set; }
    public ICollection<Solicitud> Solicitudes { get; private set; }

    public bool CanEliminar => !Solicitudes.Any();

    public static Empresa CreateInstance(string nombre)
    {
        return new Empresa { Nombre = nombre, ConfiguracionGeneral = new ConfiguracionGeneral(), Solicitudes = new HashSet<Solicitud>() };
    }

    public void SetNombre(string nombre)
    {
        Nombre = nombre;
    }
}
