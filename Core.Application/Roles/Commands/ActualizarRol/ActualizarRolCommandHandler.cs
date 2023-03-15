using System.Data.Entity;
using System.Data.Entity.Core;
using Common.Models;
using Core.Domain.Entities;
using Infrastructure.Persistance;
using MediatR;

namespace Core.Application.Roles.Commands.ActualizarRol;

public sealed class ActualizarRolCommandHandler : IRequestHandler<ActualizarRolCommand>
{
    private readonly ManejadorDocumentosCfdiDbContext _context;

    public ActualizarRolCommandHandler(ManejadorDocumentosCfdiDbContext context)
    {
        _context = context;
    }

    public async Task Handle(ActualizarRolCommand request, CancellationToken cancellationToken)
    {
        Rol rol = await _context.Roles.SingleOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

        if (rol == null)
            throw new ObjectNotFoundException($"No se encontrol el rol con id {request.Id}.");

        rol.ActualizarRol(request.Nombre, request.Descripcion);
        rol.ActualizarPermisos(request.Permisos.Select(p => p.PermisoAplicacion).PackPermissionsIntoString());

        await _context.SaveChangesAsync(cancellationToken);
    }
}
