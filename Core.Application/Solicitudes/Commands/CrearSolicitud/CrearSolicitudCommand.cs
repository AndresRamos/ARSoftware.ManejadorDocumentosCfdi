using MediatR;

namespace Core.Application.Solicitudes.Commands.CrearSolicitud;

public sealed record CrearSolicitudCommand(int EmpresaId,
                                           int UsuarioId,
                                           DateTime FechaInicio,
                                           DateTime FechaFin,
                                           string RfcEmisor,
                                           string RfcReceptor,
                                           string RfcSolicitante,
                                           string TipoSolicitud,
                                           string Uuid) : IRequest<int>;
