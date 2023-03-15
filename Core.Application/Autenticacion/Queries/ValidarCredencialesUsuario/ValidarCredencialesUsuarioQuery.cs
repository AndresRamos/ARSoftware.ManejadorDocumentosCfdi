using Core.Application.Usuarios.Models;
using MediatR;

namespace Core.Application.Autenticacion.Queries.ValidarCredencialesUsuario;

public sealed record ValidarCredencialesUsuarioQuery(string NombreUsuario, string Contrasena) : IRequest<UsuarioDto>;
