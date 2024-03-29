﻿using System.Data.Entity;
using System.Data.Entity.Core;
using Core.Domain.Entities;
using Infrastructure.Persistance;
using MediatR;

namespace Core.Application.Empresas.Commands.ActualizarEmpresaPerfil;

public sealed class ActualizarEmpresaPerfilCommandHandler : IRequestHandler<ActualizarEmpresaPerfilCommand>
{
    private readonly ManejadorDocumentosCfdiDbContext _context;

    public ActualizarEmpresaPerfilCommandHandler(ManejadorDocumentosCfdiDbContext context)
    {
        _context = context;
    }

    public async Task Handle(ActualizarEmpresaPerfilCommand request, CancellationToken cancellationToken)
    {
        Empresa empresa = await _context.Empresas.SingleOrDefaultAsync(e => e.Id == request.EmpresaId, cancellationToken);

        if (empresa is null)
            throw new ObjectNotFoundException($"No se encontro la empresa con id {request.EmpresaId}.");

        empresa.SetNombre(request.Nombre);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
