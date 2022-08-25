using System.Data.Entity;
using System.Data.Entity.Core;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ARSoftware.Cfdi.DescargaMasiva.Enumerations;
using Core.Application.Paquetes.Models;
using Core.Application.Solicitudes.Models;
using Core.Domain.Entities;
using Infrastructure.Persistance;
using MediatR;

namespace Core.Application.Solicitudes.Queries.BuscarSolicitudPorId;

public class BuscarSolicitudPorIdQueryHandler : IRequestHandler<BuscarSolicitudPorIdQuery, SolicitudDto>
{
    private readonly ManejadorDocumentosCfdiDbContext _context;

    public BuscarSolicitudPorIdQueryHandler(ManejadorDocumentosCfdiDbContext context)
    {
        _context = context;
    }

    public async Task<SolicitudDto> Handle(BuscarSolicitudPorIdQuery request, CancellationToken cancellationToken)
    {
        Solicitud solicitud = await _context.Solicitudes.Include(s => s.SolicitudAutenticacion)
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

        return new SolicitudDto(solicitud.Id,
            solicitud.FechaCreacionUtc,
            solicitud.FechaInicio,
            solicitud.FechaFin,
            solicitud.RfcEmisor,
            solicitud.RfcReceptor,
            solicitud.RfcSolicitante,
            solicitud.TipoSolicitud,
            solicitud.SolicitudAutenticacion != null
                ? new SolicitudAutenticacionDto(solicitud.SolicitudAutenticacion.Id,
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
                ? new SolicitudSolicitudDto(solicitud.SolicitudSolicitud.Id,
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
                ? new SolicitudVerificacionDto(solicitud.SolicitudVerificacion.Id,
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
                    CodigoEstatusSolicitud.TryFromName(solicitud.SolicitudVerificacion.CodEstatus,
                        out CodigoEstatusSolicitud codigoEstatusSolicitudVer)
                        ? new CodigoEstatusSolicitudDto(codigoEstatusSolicitudVer.Value,
                            codigoEstatusSolicitudVer.Name,
                            codigoEstatusSolicitudVer.Mensaje,
                            codigoEstatusSolicitudVer.Observaciones)
                        : null,
                    EstadoSolicitud.TryFromValue(int.Parse(solicitud.SolicitudVerificacion.EstadoSolicitud),
                        out EstadoSolicitud estadoSolicitudVer)
                        ? new EstadoSolicitudDto(estadoSolicitudVer.Value, estadoSolicitudVer.Name)
                        : null,
                    CodigoEstadoSolicitud.TryFromName(solicitud.SolicitudVerificacion.CodigoEstadoSolicitud,
                        out CodigoEstadoSolicitud codigoEstadoSolicitudVer)
                        ? new CodigoEstadoSolicitudDto(codigoEstadoSolicitudVer.Value,
                            codigoEstadoSolicitudVer.Name,
                            codigoEstadoSolicitudVer.Mensaje,
                            codigoEstadoSolicitudVer.Observaciones)
                        : null)
                : null,
            solicitud.SolicitudDescarga != null
                ? new SolicitudDescargaDto(solicitud.SolicitudDescarga.Id,
                    solicitud.SolicitudDescarga.FechaCreacionUtc,
                    solicitud.SolicitudDescarga.CodEstatus,
                    solicitud.SolicitudDescarga.Mensaje,
                    solicitud.SolicitudDescarga.PaqueteId,
                    solicitud.SolicitudDescarga.Paquete,
                    solicitud.SolicitudDescarga.Error,
                    solicitud.SolicitudDescarga.Solicitud,
                    solicitud.SolicitudDescarga.Respuesta,
                    CodigoEstatusSolicitud.TryFromName(solicitud.SolicitudDescarga.CodEstatus,
                        out CodigoEstatusSolicitud codigoEstatusSolicituddes)
                        ? new CodigoEstatusSolicitudDto(codigoEstatusSolicituddes.Value,
                            codigoEstatusSolicituddes.Name,
                            codigoEstatusSolicituddes.Mensaje,
                            codigoEstatusSolicituddes.Observaciones)
                        : null)
                : null,
            solicitud.SolicitudesWeb.OfType<SolicitudAutenticacion>()
                .OrderBy(s => s.FechaCreacionUtc)
                .ToList()
                .Select(s => new SolicitudAutenticacionDto(s.Id,
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
            solicitud.SolicitudesWeb.OfType<SolicitudSolicitud>()
                .OrderBy(s => s.FechaCreacionUtc)
                .ToList()
                .Select(s => new SolicitudSolicitudDto(s.Id,
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
            solicitud.SolicitudesWeb.OfType<SolicitudVerificacion>()
                .OrderBy(s => s.FechaCreacionUtc)
                .ToList()
                .Select(s => new SolicitudVerificacionDto(s.Id,
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
                    CodigoEstatusSolicitud.TryFromName(s.CodEstatus, out CodigoEstatusSolicitud codigoEstatusSolicitudVer2)
                        ? new CodigoEstatusSolicitudDto(codigoEstatusSolicitudVer2.Value,
                            codigoEstatusSolicitudVer2.Name,
                            codigoEstatusSolicitudVer2.Mensaje,
                            codigoEstatusSolicitudVer2.Observaciones)
                        : null,
                    EstadoSolicitud.TryFromValue(int.Parse(s.EstadoSolicitud), out EstadoSolicitud estadoSolicitudVer2)
                        ? new EstadoSolicitudDto(estadoSolicitudVer2.Value, estadoSolicitudVer2.Name)
                        : null,
                    CodigoEstadoSolicitud.TryFromName(s.CodigoEstadoSolicitud, out CodigoEstadoSolicitud codigoEstadoSolicitudVer2)
                        ? new CodigoEstadoSolicitudDto(codigoEstadoSolicitudVer2.Value,
                            codigoEstadoSolicitudVer2.Name,
                            codigoEstadoSolicitudVer2.Mensaje,
                            codigoEstadoSolicitudVer2.Observaciones)
                        : null))
                .ToList(),
            solicitud.SolicitudesWeb.OfType<SolicitudDescarga>()
                .OrderBy(s => s.FechaCreacionUtc)
                .ToList()
                .Select(s => new SolicitudDescargaDto(s.Id,
                    s.FechaCreacionUtc,
                    s.CodEstatus,
                    s.Mensaje,
                    s.PaqueteId,
                    s.Paquete,
                    s.Error,
                    s.Solicitud,
                    s.Respuesta,
                    CodigoEstatusSolicitud.TryFromName(s.CodEstatus, out CodigoEstatusSolicitud codigoEstatusSolicituddes2)
                        ? new CodigoEstatusSolicitudDto(codigoEstatusSolicituddes2.Value,
                            codigoEstatusSolicituddes2.Name,
                            codigoEstatusSolicituddes2.Mensaje,
                            codigoEstatusSolicituddes2.Observaciones)
                        : null))
                .ToList(),
            solicitud.Paquetes.Select(p => new PaqueteDto(p.Id, p.IdSat, p.Contenido)).ToList());
    }
}
