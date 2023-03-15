using Core.Application.Usuarios.Models;
using MediatR;

namespace Core.Application.Usuarios.Queries.BuscarUsuarioPorId;

public sealed record BuscarUsuarioPorIdQuery(int UsuarioId) : IRequest<UsuarioDto>;
