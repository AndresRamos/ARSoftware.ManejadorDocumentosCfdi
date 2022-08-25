using System;

namespace Core.Application.Solicitudes.Models;

public class SolicitudAutenticacionDto
{
    public SolicitudAutenticacionDto(int id,
                                     DateTime fechaCreacionUtc,
                                     DateTime fechaTokenCreacionUtc,
                                     DateTime fechaTokenExpiracionUtc,
                                     string token,
                                     string autorizacion,
                                     string faultCode,
                                     string faultString,
                                     string error,
                                     string solicitud,
                                     string respuesta)
    {
        Id = id;
        FechaCreacionUtc = fechaCreacionUtc;
        FechaCreacionLocal = fechaCreacionUtc.ToLocalTime();
        FechaTokenCreacionUtc = fechaTokenCreacionUtc;
        FechaTokenCreacionLocal = fechaTokenCreacionUtc.ToLocalTime();
        FechaTokenExpiracionUtc = fechaTokenExpiracionUtc;
        FechaTokenExpiracionLocal = fechaTokenExpiracionUtc.ToLocalTime();
        Token = token;
        Autorizacion = autorizacion;
        FaultCode = faultCode;
        FaultString = faultString;
        Error = error;
        Solicitud = solicitud;
        Respuesta = respuesta;
    }

    public int Id { get; set; }
    public DateTime FechaCreacionUtc { get; set; }
    public DateTime FechaCreacionLocal { get; set; }
    public DateTime FechaTokenCreacionUtc { get; set; }
    public DateTime FechaTokenCreacionLocal { get; set; }
    public DateTime FechaTokenExpiracionUtc { get; set; }
    public DateTime FechaTokenExpiracionLocal { get; set; }
    public string Token { get; set; }
    public string Autorizacion { get; set; }
    public string FaultCode { get; set; }
    public string FaultString { get; set; }
    public string Error { get; set; }
    public string Solicitud { get; set; }
    public string Respuesta { get; set; }
}
