using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Common.Models;

public static class PermisosHelper
{
    public static string PackPermissionsIntoString(this IEnumerable<PermisosAplicacion> permissions)
    {
        return permissions.Aggregate("", (s, permission) => s + (char)permission);
    }

    public static IEnumerable<PermisosAplicacion> UnpackPermissionsFromString(this string packedPermissions)
    {
        if (packedPermissions == null)
            throw new ArgumentNullException(nameof(packedPermissions));

        foreach (char character in packedPermissions)
            yield return (PermisosAplicacion)character;
    }

    public static PermisosAplicacion? BuscarPermisoPorNombre(this string permissionName)
    {
        return Enum.TryParse(permissionName, out PermisosAplicacion permission) ? permission : null;
    }

    public static PermisoAplicacionDto ToPermisoDto(this PermisosAplicacion permisoApplicacion)
    {
        Type enumType = typeof(PermisosAplicacion);

        MemberInfo[] member = enumType.GetMember(permisoApplicacion.ToString());
        var displayAttribute = member[0].GetCustomAttribute<DisplayAttribute>();
        if (displayAttribute is null)
            throw new InvalidOperationException($"Display attribute not found for permiso {permisoApplicacion}");

        var permiso = (PermisosAplicacion)Enum.Parse(enumType, permisoApplicacion.ToString(), false);

        return new PermisoAplicacionDto(permiso,
            displayAttribute.Name ?? throw new InvalidOperationException("Name is null."),
            displayAttribute.GroupName ?? throw new InvalidOperationException("Group name is null."),
            displayAttribute.Description ?? throw new InvalidOperationException("Description is null."));
    }

    public static IEnumerable<PermisoAplicacionDto> UnpackToPermisosDto(this string packedPermissions)
    {
        return packedPermissions.UnpackPermissionsFromString().Select(p => p.ToPermisoDto()).ToList();
    }

    public static bool UsuarioTieneEstePermiso(this PermisosAplicacion[] permisosUsuario, PermisosAplicacion permisoAValidar)
    {
        return permisosUsuario.Contains(permisoAValidar) || permisosUsuario.Contains(PermisosAplicacion.TodosLosPermisos);
    }
}
