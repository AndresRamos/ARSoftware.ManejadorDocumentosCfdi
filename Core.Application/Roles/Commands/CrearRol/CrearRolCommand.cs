using Common.Models;
using MediatR;

namespace Core.Application.Roles.Commands.CrearRol;

public sealed record CrearRolCommand(string Nombre, string Descripcion, IEnumerable<PermisoAplicacionDto> Permisos) : IRequest<int>;
