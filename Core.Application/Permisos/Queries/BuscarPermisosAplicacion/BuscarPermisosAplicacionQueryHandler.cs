using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Common.Models;
using MediatR;

namespace Core.Application.Permisos.Queries.BuscarPermisosAplicacion
{
    public class BuscarPermisosAplicacionQueryHandler : IRequestHandler<BuscarPermisosAplicacionQuery, IEnumerable<PermisoAplicacionDto>>
    {
        public Task<IEnumerable<PermisoAplicacionDto>> Handle(BuscarPermisosAplicacionQuery request, CancellationToken cancellationToken)
        {
            var permisos = new List<PermisoAplicacionDto>();

            Type enumType = typeof(PermisosAplicacion);

            foreach (string permissionName in Enum.GetNames(enumType))
            {
                MemberInfo[] member = enumType.GetMember(permissionName);

                var displayAttribute = member[0].GetCustomAttribute<DisplayAttribute>();
                if (displayAttribute == null)
                {
                    continue;
                }

                var permiso = (PermisosAplicacion)Enum.Parse(enumType, permissionName, false);

                permisos.Add(new PermisoAplicacionDto(permiso,
                    displayAttribute.Name,
                    displayAttribute.GroupName,
                    displayAttribute.Description));
            }

            return Task.FromResult<IEnumerable<PermisoAplicacionDto>>(permisos);
        }
    }
}
