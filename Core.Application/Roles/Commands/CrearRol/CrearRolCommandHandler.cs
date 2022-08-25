using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.Models;
using Core.Domain.Entities;
using Infrastructure.Persistance;
using MediatR;

namespace Core.Application.Roles.Commands.CrearRol;

public class CrearRolCommandHandler : IRequestHandler<CrearRolCommand, int>
{
    private readonly ManejadorDocumentosCfdiDbContext _context;

    public CrearRolCommandHandler(ManejadorDocumentosCfdiDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CrearRolCommand request, CancellationToken cancellationToken)
    {
        var rol = Rol.CreateInstance(request.Nombre,
            request.Descripcion,
            request.Permisos.Select(p => p.PermisoAplicacion).PackPermissionsIntoString());

        _context.Roles.Add(rol);

        await _context.SaveChangesAsync(cancellationToken);

        return rol.Id;
    }
}
