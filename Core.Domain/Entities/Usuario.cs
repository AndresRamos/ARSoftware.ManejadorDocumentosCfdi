using System.Collections.Generic;
using System.Linq;
using Core.Application.Permisos.Helpers;
using Core.Application.Permisos.Models;

namespace Core.Domain.Entities
{
    public class Usuario
    {
        private Usuario()
        {
        }

        public int Id { get; private set; }
        public string PrimerNombre { get; private set; }
        public string Apellido { get; private set; }
        public string Email { get; private set; }
        public string NombreUsuario { get; private set; }
        public string PasswordHash { get; private set; }
        public string PasswordSalt { get; private set; }
        public ICollection<Rol> Roles { get; private set; }
        public ICollection<Empresa> EmpresasPermitidas { get; private set; }
        public ICollection<Solicitud> Solicitudes { get; private set; }

        public static Usuario CreateInstance(string primerNombre, string apellido, string email, string nombreUsuario, string passwordHash, string passwordSalt)
        {
            return new Usuario
            {
                PrimerNombre = primerNombre,
                Apellido = apellido,
                Email = email,
                NombreUsuario = nombreUsuario,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Roles = new HashSet<Rol>(),
                EmpresasPermitidas = new HashSet<Empresa>(),
                Solicitudes = new HashSet<Solicitud>()
            };
        }

        public void ActualizarPerfil(string primerNombre, string apellido, string email, string nombreUsuario)
        {
            PrimerNombre = primerNombre;
            Apellido = apellido;
            Email = email;
            NombreUsuario = nombreUsuario;
        }

        public void CambiarContrasena(string passwordHash, string passwordSalt)
        {
            PasswordHash = passwordHash;
            PasswordSalt = passwordSalt;
        }

        public void AgregarEmpresaPermitida(Empresa empresa)
        {
            EmpresasPermitidas.Add(empresa);
        }

        public void RemoverEmpresaPermitida(Empresa empresa)
        {
            EmpresasPermitidas.Remove(empresa);
        }

        public bool TienePermiso(PermisosAplicacion permiso)
        {
            var permisosList = new List<PermisosAplicacion>();

            foreach (var permisosString in Roles.Select(r => r.Permisos))
            {
                permisosList.AddRange(permisosString.UnpackPermissionsFromString());
            }

            return permisosList.Distinct().ToArray().UsuarioTieneEstePermiso(permiso);
        }
    }
}