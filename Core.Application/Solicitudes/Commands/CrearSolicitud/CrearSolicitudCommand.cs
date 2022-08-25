using System;
using MediatR;

namespace Core.Application.Solicitudes.Commands.CrearSolicitud;

public class CrearSolicitudCommand : IRequest<int>
{
    public CrearSolicitudCommand(int empresaId,
                                 int usuarioId,
                                 DateTime fechaInicio,
                                 DateTime fechaFin,
                                 string rfcEmisor,
                                 string rfcReceptor,
                                 string rfcSolicitante,
                                 string tipoSolicitud)
    {
        EmpresaId = empresaId;
        UsuarioId = usuarioId;
        FechaInicio = fechaInicio;
        FechaFin = fechaFin;
        RfcEmisor = rfcEmisor;
        RfcReceptor = rfcReceptor;
        RfcSolicitante = rfcSolicitante;
        TipoSolicitud = tipoSolicitud;
    }

    public int EmpresaId { get; }
    public int UsuarioId { get; }
    public DateTime FechaInicio { get; }
    public DateTime FechaFin { get; }
    public string RfcEmisor { get; }
    public string RfcReceptor { get; }
    public string RfcSolicitante { get; }
    public string TipoSolicitud { get; }
}
