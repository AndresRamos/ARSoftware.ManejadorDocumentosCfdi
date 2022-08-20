using System.Collections.Generic;
using Common.Models;
using MediatR;

namespace Core.Application.Roles.Commands.ActualizarRol
{
    public class ActualizarRolCommand : IRequest
    {
        public ActualizarRolCommand(int id, string nombre, string descripcion, IEnumerable<PermisoAplicacionDto> permisos)
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
}
