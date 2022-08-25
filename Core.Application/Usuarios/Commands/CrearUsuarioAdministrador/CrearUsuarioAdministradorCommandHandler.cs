using System.Collections.Generic;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using Common.Infrastructure;
using Common.Models;
using Core.Domain.Entities;
using Infrastructure.Persistance;
using MediatR;

namespace Core.Application.Usuarios.Commands.CrearUsuarioAdministrador;

public class CrearUsuarioAdministradorCommandHandler : IRequestHandler<CrearUsuarioAdministradorCommand, Unit>
{
    private readonly ManejadorDocumentosCfdiDbContext _context;

    public CrearUsuarioAdministradorCommandHandler(ManejadorDocumentosCfdiDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(CrearUsuarioAdministradorCommand request, CancellationToken cancellationToken)
    {
        const string rolAdministradorNombre = "Administrador";
        Rol rolAdministrador = await _context.Roles.SingleOrDefaultAsync(r => r.Nombre == rolAdministradorNombre, cancellationToken);
        if (rolAdministrador == null)
        {
            var permisos = new List<PermisosAplicacion> { PermisosAplicacion.TodosLosPermisos };
            rolAdministrador = Rol.CreateInstance("Administrador", "Rol de administrador", permisos.PackPermissionsIntoString());
            _context.Roles.Add(rolAdministrador);
            await _context.SaveChangesAsync(cancellationToken);
        }

        const string usuarioAdministradorNombreUsuario = "admin";
        Usuario usuarioAdministrador =
            await _context.Usuarios.SingleOrDefaultAsync(u => u.NombreUsuario == usuarioAdministradorNombreUsuario, cancellationToken);

        if (usuarioAdministrador == null)
        {
            byte[] passwordSalt = PasswordHasher.CreateSalt();
            byte[] passwordHash = PasswordHasher.CreateHash("admin", passwordSalt);
            usuarioAdministrador = Usuario.CreateInstance("Admin",
                "Admin",
                "",
                usuarioAdministradorNombreUsuario,
                PasswordHasher.GetHashString(passwordHash),
                PasswordHasher.GetHashString(passwordSalt));

            _context.Usuarios.Add(usuarioAdministrador);
            await _context.SaveChangesAsync(cancellationToken);

            usuarioAdministrador.Roles.Add(rolAdministrador);
            await _context.SaveChangesAsync(cancellationToken);
        }

        return Unit.Value;
    }
}
