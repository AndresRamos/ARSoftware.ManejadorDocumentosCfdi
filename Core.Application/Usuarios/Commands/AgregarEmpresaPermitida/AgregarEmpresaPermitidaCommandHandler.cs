using System.Data.Entity;
using System.Data.Entity.Core;
using Core.Domain.Entities;
using Infrastructure.Persistance;
using MediatR;

namespace Core.Application.Usuarios.Commands.AgregarEmpresaPermitida;

public class AgregarEmpresaPermitidaCommandHandler : IRequestHandler<AgregarEmpresaPermitidaCommand>
{
    private readonly ManejadorDocumentosCfdiDbContext _context;

    public AgregarEmpresaPermitidaCommandHandler(ManejadorDocumentosCfdiDbContext context)
    {
        _context = context;
    }

    public async Task Handle(AgregarEmpresaPermitidaCommand request, CancellationToken cancellationToken)
    {
        Usuario usuario = await _context.Usuarios.Include(u => u.EmpresasPermitidas)
            .SingleOrDefaultAsync(u => u.Id == request.UsuarioId, cancellationToken);

        if (usuario is null)
            throw new ObjectNotFoundException($"No se encontro le usuario con id {request.UsuarioId}.");

        Empresa empresa = await _context.Empresas.SingleOrDefaultAsync(e => e.Id == request.EmpresaId, cancellationToken);
        if (empresa is null)
            throw new ObjectNotFoundException($"No se encontro la empresa con id {request.EmpresaId}.");

        usuario.AgregarEmpresaPermitida(empresa);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
