namespace Common.Models;

public sealed class PermisoAplicacionDto
{
    public PermisoAplicacionDto(PermisosAplicacion permisoAplicacion, string nombre, string grupo, string descripcion)
    {
        PermisoAplicacion = permisoAplicacion;
        Nombre = nombre;
        Grupo = grupo;
        Descripcion = descripcion;
    }

    public PermisosAplicacion PermisoAplicacion { get; set; }
    public string Nombre { get; set; }
    public string Descripcion { get; set; }
    public string Grupo { get; set; }
}
