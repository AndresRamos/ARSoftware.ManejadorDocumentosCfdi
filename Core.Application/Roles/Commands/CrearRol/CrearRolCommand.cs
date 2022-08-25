using System.Collections.Generic;
using Common.Models;
using MediatR;

namespace Core.Application.Roles.Commands.CrearRol;

public class CrearRolCommand : IRequest<int>
{
    public CrearRolCommand(string nombre, string descripcion, IEnumerable<PermisoAplicacionDto> permisos)
    {
        Nombre = nombre;
        Descripcion = descripcion;
        Permisos = permisos;
    }

    public string Nombre { get; }
    public string Descripcion { get; }
    public IEnumerable<PermisoAplicacionDto> Permisos { get; }
}
