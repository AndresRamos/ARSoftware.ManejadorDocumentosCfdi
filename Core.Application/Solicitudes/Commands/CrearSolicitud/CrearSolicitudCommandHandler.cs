﻿using System.Data.Entity;
using System.Data.Entity.Core;
using Common.Models;
using Core.Domain.Entities;
using Infrastructure.Persistance;
using MediatR;

namespace Core.Application.Solicitudes.Commands.CrearSolicitud;

public sealed class CrearSolicitudCommandHandler : IRequestHandler<CrearSolicitudCommand, int>
{
    private readonly ManejadorDocumentosCfdiDbContext _context;

    public CrearSolicitudCommandHandler(ManejadorDocumentosCfdiDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CrearSolicitudCommand request, CancellationToken cancellationToken)
    {
        Usuario usuario = await _context.Usuarios.Include(u => u.Roles)
            .SingleOrDefaultAsync(u => u.Id == request.UsuarioId, cancellationToken);

        if (usuario is null)
            throw new ObjectNotFoundException("No se encontro el usuario.");

        if (!usuario.TienePermiso(PermisosAplicacion.PuedeCrearSolicitud))
            throw new InvalidOperationException("El usuario no tiene permiso de crear solicitudes.");

        var nuevaSolicitud = Solicitud.CreateNew(request.EmpresaId,
            request.UsuarioId,
            request.FechaInicio,
            request.FechaFin,
            request.RfcEmisor,
            request.RfcReceptor,
            request.RfcSolicitante,
            request.TipoSolicitud,
            request.Uuid);

        _context.Entry(nuevaSolicitud).State = EntityState.Added;

        await _context.SaveChangesAsync(cancellationToken);

        return nuevaSolicitud.Id;
    }
}
