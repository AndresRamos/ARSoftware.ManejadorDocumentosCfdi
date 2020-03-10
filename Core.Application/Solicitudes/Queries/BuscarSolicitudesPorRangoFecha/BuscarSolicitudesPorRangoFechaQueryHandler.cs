using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ARSoftware.Cfdi.DescargaMasiva.Enumerations;
using Core.Application.Paquetes.Models;
using Core.Application.Solicitudes.Models;
using Core.Domain.Entities;
using Infrastructure.Persistance;
using MediatR;

namespace Core.Application.Solicitudes.Queries.BuscarSolicitudesPorRangoFecha
{
    public class BuscarSolicitudesPorRangoFechaQueryHandler : IRequestHandler<BuscarSolicitudesPorRangoFechaQuery, IEnumerable<SolicitudDto>>
    {
        private readonly ManejadorDocumentosCfdiDbContext _context;

        public BuscarSolicitudesPorRangoFechaQueryHandler(ManejadorDocumentosCfdiDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SolicitudDto>> Handle(BuscarSolicitudesPorRangoFechaQuery request, CancellationToken cancellationToken)
        {
            var fechaInicio = new DateTime(request.FechaInicio.Year, request.FechaInicio.Month, request.FechaInicio.Day, 0, 0, 0).ToUniversalTime();
            var fechaFin = new DateTime(request.FechaFin.Year, request.FechaFin.Month, request.FechaFin.Day, 23, 59, 59).ToUniversalTime();

            var solicitudes = await _context.Solicitudes
                .Include(s => s.SolicitudAutenticacion)
                .Include(s => s.SolicitudSolicitud)
                .Include(s => s.SolicitudVerificacion.PaquetesIds)
                .Include(s => s.SolicitudDescarga)
                .Include(s => s.SolicitudesWeb)
                .Include(s => s.Paquetes)
                .Where(s => s.FechaCreacionUtc >= fechaInicio && s.FechaCreacionUtc <= fechaFin)
                .ToListAsync(cancellationToken);

            return solicitudes.Select(solicitud => new SolicitudDto(
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
                            solicitud.SolicitudVerificacion.PaquetesIds.Select(p => new PaqueteIdDto(p.Id, p.IdPaquete, p.IsDescargado)).ToList(),
                            solicitud.SolicitudVerificacion.Error,
                            solicitud.SolicitudVerificacion.Solicitud,
                            solicitud.SolicitudVerificacion.Respuesta,
                            CodigoEstatusSolicitud.TryParse(solicitud.SolicitudVerificacion.CodEstatus, out var codigoEstatusSolicitudVer)
                                ? new CodigoEstatusSolicitudDto(codigoEstatusSolicitudVer.Id, codigoEstatusSolicitudVer.Name, codigoEstatusSolicitudVer.Mensaje, codigoEstatusSolicitudVer.Observaciones)
                                : null,
                            EstadoSolicitud.TryParse(solicitud.SolicitudVerificacion.EstadoSolicitud, out var estadoSolicitudVer)
                                ? new EstadoSolicitudDto(estadoSolicitudVer.Id, estadoSolicitudVer.Name)
                                                                                                                                        : null,
                            CodigoEstadoSolicitud.TryParse(solicitud.SolicitudVerificacion.CodigoEstadoSolicitud, out var codigoEstadoSolicitudVer)
                                ? new CodigoEstadoSolicitudDto(codigoEstadoSolicitudVer.Id, codigoEstadoSolicitudVer.Name, codigoEstadoSolicitudVer.Mensaje, codigoEstadoSolicitudVer.Observaciones)
                                : null)
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
                            solicitud.SolicitudDescarga.Respuesta,
                            CodigoEstatusSolicitud.TryParse(solicitud.SolicitudDescarga.CodEstatus, out var codigoEstatusSolicituddes)
                                ? new CodigoEstatusSolicitudDto(codigoEstatusSolicituddes.Id, codigoEstatusSolicituddes.Name, codigoEstatusSolicituddes.Mensaje, codigoEstatusSolicituddes.Observaciones)
                                : null)
                        : null,
                    solicitud.SolicitudesWeb.OfType<SolicitudAutenticacion>().ToList()
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
                    solicitud.SolicitudesWeb.OfType<SolicitudSolicitud>().ToList()
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
                    solicitud.SolicitudesWeb.OfType<SolicitudVerificacion>().ToList()
                        .Select(s => new SolicitudVerificacionDto(
                            s.Id,
                            s.FechaCreacionUtc,
                            s.CodEstatus,
                            s.Mensaje,
                            s.CodigoEstadoSolicitud,
                            s.EstadoSolicitud,
                            s.NumeroCfdis,
                            s.PaquetesIds.Select(p => new PaqueteIdDto(p.Id, p.IdPaquete, p.IsDescargado)).ToList(),
                            s.Error,
                            s.Solicitud,
                            s.Respuesta,
                            CodigoEstatusSolicitud.TryParse(s.CodEstatus, out var codigoEstatusSolicitudVer2)
                                ? new CodigoEstatusSolicitudDto(codigoEstatusSolicitudVer2.Id, codigoEstatusSolicitudVer2.Name, codigoEstatusSolicitudVer2.Mensaje, codigoEstatusSolicitudVer2.Observaciones)
                                : null,
                            EstadoSolicitud.TryParse(s.EstadoSolicitud, out var estadoSolicitudVer2)
                                ? new EstadoSolicitudDto(estadoSolicitudVer2.Id, estadoSolicitudVer2.Name)
                                : null,
                            CodigoEstadoSolicitud.TryParse(s.CodigoEstadoSolicitud, out var codigoEstadoSolicitudVer2)
                                ? new CodigoEstadoSolicitudDto(codigoEstadoSolicitudVer2.Id, codigoEstadoSolicitudVer2.Name, codigoEstadoSolicitudVer2.Mensaje, codigoEstadoSolicitudVer2.Observaciones)
                                : null))
                        .ToList(),
                    solicitud.SolicitudesWeb.OfType<SolicitudDescarga>().ToList()
                        .Select(s => new SolicitudDescargaDto(
                            s.Id,
                            s.FechaCreacionUtc,
                            s.CodEstatus,
                            s.Mensaje,
                            s.PaqueteId,
                            s.Paquete,
                            s.Error,
                            s.Solicitud,
                            s.Respuesta,
                            CodigoEstatusSolicitud.TryParse(s.CodEstatus, out var codigoEstatusSolicituddes2)
                                ? new CodigoEstatusSolicitudDto(codigoEstatusSolicituddes2.Id, codigoEstatusSolicituddes2.Name, codigoEstatusSolicituddes2.Mensaje, codigoEstatusSolicituddes2.Observaciones)
                                : null))
                        .ToList(),
                    solicitud.Paquetes.Select(p => new PaqueteDto(p.Id, p.IdSat, p.Contenido)).ToList()
                ))
                .ToList();
        }
    }
}