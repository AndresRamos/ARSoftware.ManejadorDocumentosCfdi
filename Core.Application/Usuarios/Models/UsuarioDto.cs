using System.Collections.Generic;
using System.Linq;
using Core.Application.Permisos.Helpers;
using Core.Application.Permisos.Models;
using Core.Application.Roles.Models;

namespace Core.Application.Usuarios.Models
{
    public class UsuarioDto
    {
        public UsuarioDto(int id, string primerNombre, string apellido, string email, string nombreUsuario, string passwordHash, string passwordSalt, ICollection<RolDto> roles)
        {
            Id = id;
            PrimerNombre = primerNombre;
            Apellido = apellido;
            Email = email;
            NombreUsuario = nombreUsuario;
            PasswordHash = passwordHash;
            PasswordSalt = passwordSalt;
            Roles = roles;
        }

        public int Id { get; }
        public string PrimerNombre { get; }
        public string Apellido { get; }
        public string Email { get; }
        public string NombreUsuario { get; }
        public string PasswordHash { get; }
        public string PasswordSalt { get; }
        public ICollection<RolDto> Roles { get; }
        public string NombreComleto => $"{PrimerNombre} {Apellido}";

        public bool TienePermiso(PermisosAplicacion permiso)
        {
            var permisos = Roles.SelectMany(r => r.Permisos).Select(p => p.PermisoAplicacion).Distinct().ToArray();

            return permisos.UsuarioTieneEstePermiso(permiso);
        }
    }
}