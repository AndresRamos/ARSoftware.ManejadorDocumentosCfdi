using System;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.Infrastructure;
using Common.Models;
using Core.Application.Roles.Models;
using Core.Application.Usuarios.Models;
using Core.Domain.Entities;
using Infrastructure.Persistance;
using MediatR;

namespace Core.Application.Autenticacion.Queries.ValidarCredencialesUsuario;

public class ValidarCredencialesUsuarioQueryHandler : IRequestHandler<ValidarCredencialesUsuarioQuery, UsuarioDto>
{
    private readonly ManejadorDocumentosCfdiDbContext _context;

    public ValidarCredencialesUsuarioQueryHandler(ManejadorDocumentosCfdiDbContext context)
    {
        _context = context;
    }

    public async Task<UsuarioDto> Handle(ValidarCredencialesUsuarioQuery request, CancellationToken cancellationToken)
    {
        Usuario usuario = await _context.Usuarios.Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.NombreUsuario == request.NombreUsuario, cancellationToken);
        if (usuario == null)
        {
            throw new ObjectNotFoundException($"No se encontro un usuario con el nombre de usaurio {request.NombreUsuario}.");
        }

        if (PasswordHasher.Validate(request.Contrasena,
                Convert.FromBase64String(usuario.PasswordHash),
                Convert.FromBase64String(usuario.PasswordSalt)))
        {
            return new UsuarioDto(usuario.Id,
                usuario.PrimerNombre,
                usuario.Apellido,
                usuario.Email,
                usuario.NombreUsuario,
                usuario.PasswordHash,
                usuario.PasswordSalt,
                usuario.Roles.Select(r => new RolDto(r.Id, r.Nombre, r.Descripcion, r.Permisos.UnpackToPermisosDto())).ToList());
        }

        return null;
    }
}
