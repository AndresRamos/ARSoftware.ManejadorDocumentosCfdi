﻿using System;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using Core.Domain.Entities;
using Infrastructure.Persistance;
using MediatR;

namespace Core.Application.Usuarios.Commands.ActualizarPerfilUsuario;

public class ActualizarPerfilUsuarioCommandHandler : IRequestHandler<ActualizarPerfilUsuarioCommand, Unit>
{
    private readonly ManejadorDocumentosCfdiDbContext _context;

    public ActualizarPerfilUsuarioCommandHandler(ManejadorDocumentosCfdiDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(ActualizarPerfilUsuarioCommand request, CancellationToken cancellationToken)
    {
        Usuario usuario = await _context.Usuarios.SingleOrDefaultAsync(u => u.Id == request.UsuarioId, cancellationToken);

        if (usuario is null)
        {
            throw new Exception($"No se encontro el usuario con ID {request.UsuarioId}");
        }

        usuario.ActualizarPerfil(request.PrimerNombre, request.Apellido, request.Email, request.NombreUsuario);

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
