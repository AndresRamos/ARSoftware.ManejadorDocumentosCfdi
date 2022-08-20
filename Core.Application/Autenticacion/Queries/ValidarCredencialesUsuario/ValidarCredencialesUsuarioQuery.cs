using Core.Application.Usuarios.Models;
using MediatR;

namespace Core.Application.Autenticacion.Queries.ValidarCredencialesUsuario
{
    public class ValidarCredencialesUsuarioQuery : IRequest<UsuarioDto>
    {
        public ValidarCredencialesUsuarioQuery(string nombreUsuario, string contrasena)
        {
            NombreUsuario = nombreUsuario;
            Contrasena = contrasena;
        }

        public string NombreUsuario { get; }
        public string Contrasena { get; }
    }
}
