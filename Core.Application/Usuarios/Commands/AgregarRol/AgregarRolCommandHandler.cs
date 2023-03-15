using System.Data.Entity;
using Core.Domain.Entities;
using Infrastructure.Persistance;
using MediatR;

namespace Core.Application.Usuarios.Commands.AgregarRol;

public class AgregarRolCommandHandler : IRequestHandler<AgregarRolCommand>
{
    private readonly ManejadorDocumentosCfdiDbContext _context;

    public AgregarRolCommandHandler(ManejadorDocumentosCfdiDbContext context)
    {
        _context = context;
    }

    public async Task Handle(AgregarRolCommand request, CancellationToken cancellationToken)
    {
        Usuario usuario = await _context.Usuarios.Include(u => u.Roles).FirstAsync(u => u.Id == request.UsuarioId, cancellationToken);
        Rol rol = await _context.Roles.FirstAsync(r => r.Id == request.RolId, cancellationToken);

        usuario.Roles.Add(rol);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
