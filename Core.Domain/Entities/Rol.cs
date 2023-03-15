// ReSharper disable UnusedAutoPropertyAccessor.Local

namespace Core.Domain.Entities;

public class Rol
{
    private Rol()
    {
    }

    public int Id { get; private set; }
    public string Nombre { get; private set; }
    public string Descripcion { get; private set; }
    public string Permisos { get; private set; }
    public ICollection<Usuario> Usuarios { get; private set; }

    public static Rol CreateInstance(string nombre, string descripcion, string permisos)
    {
        return new Rol { Nombre = nombre, Descripcion = descripcion, Permisos = permisos };
    }

    public void ActualizarRol(string nombre, string descripcion)
    {
        Nombre = nombre;
        Descripcion = descripcion;
    }

    public void ActualizarPermisos(string permisos)
    {
        Permisos = permisos;
    }
}
