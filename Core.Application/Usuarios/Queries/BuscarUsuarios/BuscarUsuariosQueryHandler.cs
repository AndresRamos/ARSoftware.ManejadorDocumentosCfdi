using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.Models;
using Core.Application.Roles.Models;
using Core.Application.Usuarios.Models;
using Infrastructure.Persistance;
using MediatR;

namespace Core.Application.Usuarios.Queries.BuscarUsuarios;

public class BuscarUsuariosQueryHandler : IRequestHandler<BuscarUsuariosQuery, IEnumerable<UsuarioDto>>
{
    private readonly ManejadorDocumentosCfdiDbContext _context;

    public BuscarUsuariosQueryHandler(ManejadorDocumentosCfdiDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<UsuarioDto>> Handle(BuscarUsuariosQuery request, CancellationToken cancellationToken)
    {
        return (await _context.Usuarios.Include(u => u.Roles).ToListAsync(cancellationToken)).Select(u =>
                new UsuarioDto(u.Id,
                    u.PrimerNombre,
                    u.Apellido,
                    u.Email,
                    u.NombreUsuario,
                    u.PasswordHash,
                    u.PasswordSalt,
                    u.Roles.Select(r => new RolDto(r.Id, r.Nombre, r.Descripcion, r.Permisos.UnpackToPermisosDto())).ToList()))
            .ToList();
    }
}
