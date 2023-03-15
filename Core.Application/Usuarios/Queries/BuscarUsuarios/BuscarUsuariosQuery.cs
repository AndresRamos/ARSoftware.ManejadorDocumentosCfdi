using Core.Application.Usuarios.Models;
using MediatR;

namespace Core.Application.Usuarios.Queries.BuscarUsuarios;

public sealed record BuscarUsuariosQuery : IRequest<IEnumerable<UsuarioDto>>;
