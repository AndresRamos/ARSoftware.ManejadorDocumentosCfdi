// ReSharper disable UnusedMember.Local
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace Core.Domain.Entities;

public class SolicitudAutenticacion : SolicitudWebBase
{
    private SolicitudAutenticacion()
    {
    }

    private SolicitudAutenticacion(string solicitud,
                                   string respuesta,
                                   DateTime fechaTokenCreacionUtc,
                                   DateTime fechaTokenExpiracionUtc,
                                   string token,
                                   string autorizacion,
                                   string faultCode,
                                   string faultString,
                                   string error) : base(solicitud, respuesta)
    {
        FechaCreacionUtc = DateTime.UtcNow;
        FechaTokenCreacionUtc = fechaTokenCreacionUtc;
        FechaTokenExpiracionUtc = fechaTokenExpiracionUtc;
        Token = token;
        Autorizacion = autorizacion;
        FaultCode = faultCode;
        FaultString = faultString;
        Error = error;
    }

    public DateTime FechaCreacionUtc { get; private set; }
    public DateTime FechaTokenCreacionUtc { get; private set; }
    public DateTime FechaTokenExpiracionUtc { get; private set; }
    public string Token { get; private set; }
    public string Autorizacion { get; private set; }
    public string FaultCode { get; private set; }
    public string FaultString { get; private set; }
    public string Error { get; private set; }
    public bool IsTokenValido => DateTime.UtcNow < FechaTokenExpiracionUtc;

    public static SolicitudAutenticacion CreateInstance(string solicitud,
                                                        string respuesta,
                                                        DateTime fechaTokenCreacionUtc,
                                                        DateTime fechaTokenExpiracionUtc,
                                                        string token,
                                                        string autorizacion,
                                                        string faultCode,
                                                        string faultString,
                                                        string error)
    {
        return new SolicitudAutenticacion(solicitud,
            respuesta,
            fechaTokenCreacionUtc,
            fechaTokenExpiracionUtc,
            token,
            autorizacion,
            faultCode,
            faultString,
            error);
    }
}
