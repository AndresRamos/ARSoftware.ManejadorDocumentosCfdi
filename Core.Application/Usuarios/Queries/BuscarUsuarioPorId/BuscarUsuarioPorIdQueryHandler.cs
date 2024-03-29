﻿using System.Data.Entity;
using System.Data.Entity.Core;
using Common.Models;
using Core.Application.Roles.Models;
using Core.Application.Usuarios.Models;
using Core.Domain.Entities;
using Infrastructure.Persistance;
using MediatR;

namespace Core.Application.Usuarios.Queries.BuscarUsuarioPorId;

public sealed class BuscarUsuarioPorIdQueryHandler : IRequestHandler<BuscarUsuarioPorIdQuery, UsuarioDto>
{
    private readonly ManejadorDocumentosCfdiDbContext _context;

    public BuscarUsuarioPorIdQueryHandler(ManejadorDocumentosCfdiDbContext context)
    {
        _context = context;
    }

    public async Task<UsuarioDto> Handle(BuscarUsuarioPorIdQuery request, CancellationToken cancellationToken)
    {
        Usuario usuario = await _context.Usuarios.Include(r => r.Roles)
            .SingleOrDefaultAsync(u => u.Id == request.UsuarioId, cancellationToken);

        if (usuario == null)
            throw new ObjectNotFoundException($"No se encontro el usuario con el Id {request.UsuarioId}");

        return new UsuarioDto(usuario.Id,
            usuario.PrimerNombre,
            usuario.Apellido,
            usuario.Email,
            usuario.NombreUsuario,
            usuario.PasswordHash,
            usuario.PasswordSalt,
            usuario.Roles.Select(r => new RolDto(r.Id, r.Nombre, r.Descripcion, r.Permisos.UnpackToPermisosDto())).ToList());
    }
}
