﻿using System.Data.Entity;
using System.Data.Entity.Core;
using Core.Domain.Entities;
using Infrastructure.Persistance;
using MediatR;

namespace Core.Application.Usuarios.Commands.RemoverEmpresaPermitida;

public sealed class RemoverEmpresaPermitidaCommandHandler : IRequestHandler<RemoverEmpresaPermitidaCommand>
{
    private readonly ManejadorDocumentosCfdiDbContext _context;

    public RemoverEmpresaPermitidaCommandHandler(ManejadorDocumentosCfdiDbContext context)
    {
        _context = context;
    }

    public async Task Handle(RemoverEmpresaPermitidaCommand request, CancellationToken cancellationToken)
    {
        Usuario usuario = await _context.Usuarios.Include(u => u.EmpresasPermitidas)
            .SingleOrDefaultAsync(u => u.Id == request.UsuarioId, cancellationToken);

        if (usuario is null)
            throw new ObjectNotFoundException($"No se encontro le usuario con id {request.UsuarioId}.");

        Empresa empresa = await _context.Empresas.SingleOrDefaultAsync(e => e.Id == request.EmpresaId, cancellationToken);
        if (empresa is null)
            throw new ObjectNotFoundException($"No se encontro la empresa con id {request.EmpresaId}.");

        usuario.RemoverEmpresaPermitida(empresa);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
