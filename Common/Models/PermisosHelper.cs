using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Core.Application.Permisos.Models;

namespace Core.Application.Permisos.Helpers
{
    public static class PermisosHelper
    {
        public static string PackPermissionsIntoString(this IEnumerable<PermisosAplicacion> permissions)
        {
            return permissions.Aggregate("", (s, permission) => s + (char) permission);
        }

        public static IEnumerable<PermisosAplicacion> UnpackPermissionsFromString(this string packedPermissions)
        {
            if (packedPermissions == null)
            {
                throw new ArgumentNullException(nameof(packedPermissions));
            }

            foreach (var character in packedPermissions)
            {
                yield return (PermisosAplicacion) character;
            }
        }

        public static PermisosAplicacion? BuscarPermisoPorNombre(this string permissionName)
        {
            return Enum.TryParse(permissionName, out PermisosAplicacion permission)
                ? (PermisosAplicacion?) permission
                : null;
        }

        public static PermisoAplicacionDto ToPermisoDto(this PermisosAplicacion permisoApplicacion)
        {
            var enumType = typeof(PermisosAplicacion);

            var member = enumType.GetMember(permisoApplicacion.ToString());
            var displayAttribute = member[0].GetCustomAttribute<DisplayAttribute>();
            if (displayAttribute == null)
            {
                return null;
            }

            var permiso = (PermisosAplicacion) Enum.Parse(enumType, permisoApplicacion.ToString(), false);

            return new PermisoAplicacionDto(permiso, displayAttribute.Name, displayAttribute.GroupName, displayAttribute.Description);
        }

        public static IEnumerable<PermisoAplicacionDto> UnpackToPermisosDto(this string packedPermissions)
        {
            return packedPermissions.UnpackPermissionsFromString().Select(p => p.ToPermisoDto()).Where(p => p != null).ToList();
        }

        public static bool UsuarioTieneEstePermiso(this PermisosAplicacion[] permisosUsuario, PermisosAplicacion permisoAValidar)
        {
            return permisosUsuario.Contains(permisoAValidar) || permisosUsuario.Contains(PermisosAplicacion.TodosLosPermisos);
        }
    }
}