using System.Data.Entity;
using System.Data.Entity.Core;
using Core.Domain.Entities;
using Infrastructure.Persistance;
using MediatR;

namespace Core.Application.Roles.Commands.EliminarRol;

public sealed class EliminarRolCommandHandler : IRequestHandler<EliminarRolCommand>
{
    private readonly ManejadorDocumentosCfdiDbContext _context;

    public EliminarRolCommandHandler(ManejadorDocumentosCfdiDbContext context)
    {
        _context = context;
    }

    public async Task Handle(EliminarRolCommand request, CancellationToken cancellationToken)
    {
        Rol rol = await _context.Roles.Include(r => r.Usuarios).SingleOrDefaultAsync(r => r.Id == request.RolId, cancellationToken);
        if (rol == null)
            throw new ObjectNotFoundException("No se encontro el rol a eliminar.");

        _context.Roles.Remove(rol);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
