using System.Collections.Generic;

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
                Roles = new HashSet<Rol>()
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
    }
}