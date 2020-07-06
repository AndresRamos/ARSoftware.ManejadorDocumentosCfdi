using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Persistance;
using MediatR;

namespace Core.Application.Usuarios.Commands.RemoverRol
{
    public class RemoverRolCommandHandler : IRequestHandler<RemoverRolCommand, Unit>
    {
        private readonly ManejadorDocumentosCfdiDbContext _context;

        public RemoverRolCommandHandler(ManejadorDocumentosCfdiDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(RemoverRolCommand request, CancellationToken cancellationToken)
        {
            var usuario = await _context.Usuarios.Include(u => u.Roles).SingleOrDefaultAsync(u => u.Id == request.UsuarioId, cancellationToken);
            var rol = await _context.Roles.SingleOrDefaultAsync(r => r.Id == request.RolId, cancellationToken);

            usuario.Roles.Remove(rol);

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}