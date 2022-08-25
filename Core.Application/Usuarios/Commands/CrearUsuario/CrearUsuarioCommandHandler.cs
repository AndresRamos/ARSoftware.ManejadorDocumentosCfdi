using System.Threading;
using System.Threading.Tasks;
using Common.Infrastructure;
using Core.Domain.Entities;
using Infrastructure.Persistance;
using MediatR;

namespace Core.Application.Usuarios.Commands.CrearUsuario;

public class CrearUsuarioCommandHandler : IRequestHandler<CrearUsuarioCommand, int>
{
    private readonly ManejadorDocumentosCfdiDbContext _context;

    public CrearUsuarioCommandHandler(ManejadorDocumentosCfdiDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CrearUsuarioCommand request, CancellationToken cancellationToken)
    {
        byte[] passwordSalt = PasswordHasher.CreateSalt();
        byte[] passwordHash = PasswordHasher.CreateHash(request.Password, passwordSalt);

        var nuevoUsuario = Usuario.CreateInstance(request.PrimerNombre,
            request.Apellido,
            request.Email,
            request.NombreUsuario,
            PasswordHasher.GetHashString(passwordHash),
            PasswordHasher.GetHashString(passwordSalt));

        _context.Usuarios.Add(nuevoUsuario);

        await _context.SaveChangesAsync(cancellationToken);

        return nuevoUsuario.Id;
    }
}
