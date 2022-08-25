using System.Security.Cryptography.X509Certificates;
using MediatR;

namespace Core.Application.Cfdis.Queries.ObtenerCertificado;

public class ObtenerCertificadoQuery : IRequest<X509Certificate2>
{
    public ObtenerCertificadoQuery(byte[] bytes, string password)
    {
        Bytes = bytes;
        Password = password;
    }

    public byte[] Bytes { get; }
    public string Password { get; }
}
