using Common.Models;
using MediatR;

namespace Core.Application.Roles.Commands.ActualizarRol;

public sealed record ActualizarRolCommand(int Id, string Nombre, string Descripcion, IEnumerable<PermisoAplicacionDto> Permisos) : IRequest;
