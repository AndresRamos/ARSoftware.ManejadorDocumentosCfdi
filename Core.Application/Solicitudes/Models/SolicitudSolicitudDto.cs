namespace Core.Application.Solicitudes.Models;

public sealed class SolicitudSolicitudDto
{
    public SolicitudSolicitudDto(int id,
                                 DateTime fechaCreacionUtc,
                                 DateTime fechaInicio,
                                 DateTime fechaFin,
                                 string rfcEmisor,
                                 string rfcReceptor,
                                 string rfcSolicitante,
                                 string tipoSolicitud,
                                 string codEstatus,
                                 string mensaje,
                                 string idSolicitud,
                                 string error,
                                 string solicitud,
                                 string respuesta)
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
        CodEstatus = codEstatus;
        Mensaje = mensaje;
        IdSolicitud = idSolicitud;
        Error = error;
        Solicitud = solicitud;
        Respuesta = respuesta;
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
    public string CodEstatus { get; set; }
    public string Mensaje { get; set; }
    public string IdSolicitud { get; set; }
    public string Error { get; set; }
    public string Solicitud { get; set; }
    public string Respuesta { get; set; }
}
