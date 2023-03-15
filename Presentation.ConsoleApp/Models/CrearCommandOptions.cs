using Common.DateRanges;

namespace Presentation.ConsoleApp.Models;

public class CrearCommandOptions
{
    public string UsuarioNombre { get; set; }
    public string UsuarioContrasena { get; set; }
    public string EmpresaNombre { get; set; }
    public TipoRangoFechaEnum TipoRangoFecha { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public TipoSolicitudEnum TipoSolicitud { get; set; }
    public bool Procesar { get; set; }
}
