using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using Core.Domain.Entities;
using Infrastructure.Persistance;
using MediatR;

namespace Core.Application.Usuarios.Commands.AgregarRol
{
    public class AgregarRolCommandHandler : IRequestHandler<AgregarRolCommand, Unit>
    {
        private readonly ManejadorDocumentosCfdiDbContext _context;

        public AgregarRolCommandHandler(ManejadorDocumentosCfdiDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(AgregarRolCommand request, CancellationToken cancellationToken)
        {
            Usuario usuario = await _context.Usuarios.Include(u => u.Roles).FirstAsync(u => u.Id == request.UsuarioId, cancellationToken);
            Rol rol = await _context.Roles.FirstAsync(r => r.Id == request.RolId, cancellationToken);

            usuario.Roles.Add(rol);

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
