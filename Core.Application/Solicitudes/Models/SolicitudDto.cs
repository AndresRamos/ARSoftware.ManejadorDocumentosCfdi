using Core.Application.Paquetes.Models;

namespace Core.Application.Solicitudes.Models;

public sealed class SolicitudDto
{
    public SolicitudDto(int id,
                        DateTime fechaCreacionUtc,
                        DateTime fechaInicio,
                        DateTime fechaFin,
                        string rfcEmisor,
                        string rfcReceptor,
                        string rfcSolicitante,
                        string tipoSolicitud,
                        string uuid,
                        SolicitudAutenticacionDto solicitudAutenticacion,
                        SolicitudSolicitudDto solicitudSolicitud,
                        SolicitudVerificacionDto solicitudVerificacion,
                        SolicitudDescargaDto solicitudDescarga,
                        List<SolicitudAutenticacionDto> solicitudesAutenticacion,
                        List<SolicitudSolicitudDto> solicitudesSolicitud,
                        List<SolicitudVerificacionDto> solicitudesVerificacion,
                        List<SolicitudDescargaDto> solicitudesDescarga,
                        List<PaqueteDto> paquetes)
    {
        Id = id;
        FechaCreacionUtc = fechaCreacionUtc;
        FechaCreacionLocal = fechaCreacionUtc.ToLocalTime();
        FechaInicio = fechaInicio;
        FechaFin = fechaFin;
        RfcEmisor = rfcEmisor;
        RfcReceptor = rfcReceptor;
        RfcSolicitante = rfcSolicitante;
        TipoSolicitud = tipoSolicitud;
        Uuid = uuid;
        SolicitudAutenticacion = solicitudAutenticacion;
        SolicitudSolicitud = solicitudSolicitud;
        SolicitudVerificacion = solicitudVerificacion;
        SolicitudDescarga = solicitudDescarga;
        SolicitudesAutenticacion = solicitudesAutenticacion;
        SolicitudesSolicitud = solicitudesSolicitud;
        SolicitudesVerificacion = solicitudesVerificacion;
        SolicitudesDescarga = solicitudesDescarga;
        Paquetes = paquetes;
    }

    public int Id { get; set; }
    public DateTime FechaCreacionUtc { get; set; }
    public DateTime FechaCreacionLocal { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public string RfcEmisor { get; set; }
    public string RfcReceptor { get; set; }
    public string RfcSolicitante { get; set; }
    public string TipoSolicitud { get; set; }
    public string Uuid { get; set; }
    public SolicitudAutenticacionDto SolicitudAutenticacion { get; set; }
    public SolicitudSolicitudDto SolicitudSolicitud { get; set; }
    public SolicitudVerificacionDto SolicitudVerificacion { get; set; }
    public SolicitudDescargaDto SolicitudDescarga { get; set; }
    public List<SolicitudAutenticacionDto> SolicitudesAutenticacion { get; set; }
    public List<SolicitudSolicitudDto> SolicitudesSolicitud { get; set; }
    public List<SolicitudVerificacionDto> SolicitudesVerificacion { get; set; }
    public List<SolicitudDescargaDto> SolicitudesDescarga { get; set; }
    public List<PaqueteDto> Paquetes { get; set; }
}
