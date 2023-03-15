namespace Core.Application.ConfiguracionGeneral.Models;

public sealed class CertificadoSatDto
{
    public CertificadoSatDto(byte[] certificado, string contrasena, string rfc)
    {
        Certificado = certificado;
        Contrasena = contrasena;
        Rfc = rfc;
    }

    public byte[] Certificado { get; set; }
    public string Contrasena { get; set; }
    public string Rfc { get; set; }
}
