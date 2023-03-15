using System.Data.Entity;
using Core.Domain.Entities;
using Infrastructure.Persistance;
using MediatR;

namespace Core.Application.Usuarios.Commands.RemoverRol;

public class RemoverRolCommandHandler : IRequestHandler<RemoverRolCommand>
{
    private readonly ManejadorDocumentosCfdiDbContext _context;

    public RemoverRolCommandHandler(ManejadorDocumentosCfdiDbContext context)
    {
        _context = context;
    }

    public async Task Handle(RemoverRolCommand request, CancellationToken cancellationToken)
    {
        Usuario usuario = await _context.Usuarios.Include(u => u.Roles).FirstAsync(u => u.Id == request.UsuarioId, cancellationToken);
        Rol rol = await _context.Roles.FirstAsync(r => r.Id == request.RolId, cancellationToken);

        usuario.Roles.Remove(rol);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
