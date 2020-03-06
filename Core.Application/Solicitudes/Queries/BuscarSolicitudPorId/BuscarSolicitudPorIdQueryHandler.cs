﻿using System.Data.Entity;
using System.Data.Entity.Core;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Application.Paquetes.Models;
using Core.Application.Solicitudes.Models;
using Core.Domain.Entities;
using Infrastructure.Persistance;
using MediatR;

namespace Core.Application.Solicitudes.Queries.BuscarSolicitudPorId
{
    public class BuscarSolicitudPorIdQueryHandler : IRequestHandler<BuscarSolicitudPorIdQuery, SolicitudDto>
    {
        private readonly ManejadorDocumentosCfdiDbContext _context;

        public BuscarSolicitudPorIdQueryHandler(ManejadorDocumentosCfdiDbContext context)
        {
            _context = context;
        }

        public async Task<SolicitudDto> Handle(BuscarSolicitudPorIdQuery request, CancellationToken cancellationToken)
        {
            var solicitud = await _context.Solicitudes
                .Include(s => s.SolicitudAutenticacion)
                .Include(s => s.SolicitudSolicitud)
                .Include(s => s.SolicitudVerificacion.PaquetesIds)
                .Include(s => s.SolicitudDescarga)
                .Include(s => s.SolicitudesWeb)
                .Include(s => s.Paquetes)
                .SingleOrDefaultAsync(s => s.Id == request.SolicitudId, cancellationToken);

            if (solicitud == null)
            {
                throw new ObjectNotFoundException($"No se encontro la solicitud con Id = {request.SolicitudId}");
            }
            
            return new SolicitudDto(
                solicitud.Id,
                solicitud.FechaCreacionUtc,
                solicitud.FechaInicio,
                solicitud.FechaFin,
                solicitud.RfcEmisor,
                solicitud.RfcReceptor,
                solicitud.RfcSolicitante,
                solicitud.TipoSolicitud,
                solicitud.SolicitudAutenticacion != null
                    ? new SolicitudAutenticacionDto(
                        solicitud.SolicitudAutenticacion.Id,
                        solicitud.SolicitudAutenticacion.FechaCreacionUtc,
                        solicitud.SolicitudAutenticacion.FechaTokenCreacionUtc,
                        solicitud.SolicitudAutenticacion.FechaTokenExpiracionUtc,
                        solicitud.SolicitudAutenticacion.Token,
                        solicitud.SolicitudAutenticacion.Autorizacion,
                        solicitud.SolicitudAutenticacion.FaultCode,
                        solicitud.SolicitudAutenticacion.FaultString,
                        solicitud.SolicitudAutenticacion.Error,
                        solicitud.SolicitudAutenticacion.Solicitud,
                        solicitud.SolicitudAutenticacion.Respuesta)
                    : null,
                solicitud.SolicitudSolicitud != null
                    ? new SolicitudSolicitudDto(
                        solicitud.SolicitudSolicitud.Id,
                        solicitud.SolicitudSolicitud.FechaCreacionUtc,
                        solicitud.SolicitudSolicitud.FechaInicio,
                        solicitud.SolicitudSolicitud.FechaFin,
                        solicitud.SolicitudSolicitud.RfcEmisor,
                        solicitud.SolicitudSolicitud.RfcReceptor,
                        solicitud.SolicitudSolicitud.RfcSolicitante,
                        solicitud.SolicitudSolicitud.TipoSolicitud,
                        solicitud.SolicitudSolicitud.CodEstatus,
                        solicitud.SolicitudSolicitud.Mensaje,
                        solicitud.SolicitudSolicitud.IdSolicitud,
                        solicitud.SolicitudSolicitud.Error,
                        solicitud.SolicitudSolicitud.Solicitud,
                        solicitud.SolicitudSolicitud.Respuesta)
                    : null,
                solicitud.SolicitudVerificacion != null
                    ? new SolicitudVerificacionDto(
                        solicitud.SolicitudVerificacion.Id,
                        solicitud.SolicitudVerificacion.FechaCreacionUtc,
                        solicitud.SolicitudVerificacion.CodEstatus,
                        solicitud.SolicitudVerificacion.Mensaje,
                        solicitud.SolicitudVerificacion.CodigoEstadoSolicitud,
                        solicitud.SolicitudVerificacion.EstadoSolicitud,
                        solicitud.SolicitudVerificacion.NumeroCfdis,
                        solicitud.SolicitudVerificacion.PaquetesIds.Select(p=>new PaqueteIdDto(p.Id, p.IdPaquete, p.IsDescargado)).ToList(),
                        solicitud.SolicitudVerificacion.Error,
                        solicitud.SolicitudVerificacion.Solicitud,
                        solicitud.SolicitudVerificacion.Respuesta)
                    : null,
                solicitud.SolicitudDescarga != null
                    ? new SolicitudDescargaDto(
                        solicitud.SolicitudDescarga.Id,
                        solicitud.SolicitudDescarga.FechaCreacionUtc,
                        solicitud.SolicitudDescarga.CodEstatus,
                        solicitud.SolicitudDescarga.Mensaje,
                        solicitud.SolicitudDescarga.PaqueteId,
                        solicitud.SolicitudDescarga.Paquete,
                        solicitud.SolicitudDescarga.Error,
                        solicitud.SolicitudDescarga.Solicitud,
                        solicitud.SolicitudDescarga.Respuesta)
                    : null,
                solicitud.SolicitudesWeb.OfType<SolicitudAutenticacion>().OrderBy(s => s.FechaCreacionUtc).ToList()
                    .Select(s => new SolicitudAutenticacionDto(
                        s.Id,
                        s.FechaCreacionUtc,
                        s.FechaTokenCreacionUtc,
                        s.FechaTokenExpiracionUtc,
                        s.Token,
                        s.Autorizacion,
                        s.FaultCode,
                        s.FaultString,
                        s.Error,
                        s.Solicitud,
                        s.Respuesta))
                    .ToList(),
                solicitud.SolicitudesWeb.OfType<SolicitudSolicitud>().OrderBy(s => s.FechaCreacionUtc).ToList()
                    .Select(s => new SolicitudSolicitudDto(
                        s.Id,
                        s.FechaCreacionUtc,
                        s.FechaInicio,
                        s.FechaFin,
                        s.RfcEmisor,
                        s.RfcReceptor,
                        s.RfcSolicitante,
                        s.TipoSolicitud,
                        s.CodEstatus,
                        s.Mensaje,
                        s.IdSolicitud,
                        s.Error,
                        s.Solicitud,
                        s.Respuesta))
                    .ToList(),
                solicitud.SolicitudesWeb.OfType<SolicitudVerificacion>().OrderBy(s => s.FechaCreacionUtc).ToList()
                    .Select(s => new SolicitudVerificacionDto(
                        s.Id,
                        s.FechaCreacionUtc,
                        s.CodEstatus,
                        s.Mensaje,
                        s.CodigoEstadoSolicitud,
                        s.EstadoSolicitud,
                        s.NumeroCfdis,
                        s.PaquetesIds.Select(p=>new PaqueteIdDto(p.Id, p.IdPaquete, p.IsDescargado)).ToList(),
                        s.Error,
                        s.Solicitud,
                        s.Respuesta))
                    .ToList(),
                solicitud.SolicitudesWeb.OfType<SolicitudDescarga>().OrderBy(s => s.FechaCreacionUtc).ToList()
                    .Select(s => new SolicitudDescargaDto(
                        s.Id,
                        s.FechaCreacionUtc,
                        s.CodEstatus,
                        s.Mensaje,
                        s.PaqueteId,
                        s.Paquete,
                        s.Error,
                        s.Solicitud,
                        s.Respuesta))
                    .ToList(),
                solicitud.Paquetes.Select(p => new PaqueteDto(p.Id, p.IdSat, p.Contenido)).ToList()
            );
        }
    }
}