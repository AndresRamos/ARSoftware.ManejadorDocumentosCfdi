using System.Data.Entity;
using System.Data.Entity.Core;
using System.Threading;
using System.Threading.Tasks;
using Common.Infrastructure;
using Core.Domain.Entities;
using Infrastructure.Persistance;
using MediatR;

namespace Core.Application.Usuarios.Commands.CambiarContrasena
{
    public class CambiarContrasenaCommandHandler : IRequestHandler<CambiarContrasenaCommand, Unit>
    {
        private readonly ManejadorDocumentosCfdiDbContext _context;

        public CambiarContrasenaCommandHandler(ManejadorDocumentosCfdiDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(CambiarContrasenaCommand request, CancellationToken cancellationToken)
        {
            byte[] passwordSalt = PasswordHasher.CreateSalt();
            byte[] passwordHash = PasswordHasher.CreateHash(request.PasswordNueva, passwordSalt);

            Usuario usuario = await _context.Usuarios.SingleOrDefaultAsync(u => u.Id == request.UsuarioId, cancellationToken);

            if (usuario == null)
            {
                throw new ObjectNotFoundException("No se encontro el usuario.");
            }

            usuario.CambiarContrasena(PasswordHasher.GetHashString(passwordHash), PasswordHasher.GetHashString(passwordSalt));

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
