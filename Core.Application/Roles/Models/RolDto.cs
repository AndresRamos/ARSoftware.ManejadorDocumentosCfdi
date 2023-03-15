using Common.Models;

namespace Core.Application.Roles.Models;

public sealed class RolDto
{
    public RolDto(int id, string nombre, string descripcion, IEnumerable<PermisoAplicacionDto> permisos)
    {
        Id = id;
        Nombre = nombre;
        Descripcion = descripcion;
        Permisos = permisos;
    }

    public int Id { get; }
    public string Nombre { get; }
    public string Descripcion { get; }
    public IEnumerable<PermisoAplicacionDto> Permisos { get; }
}
