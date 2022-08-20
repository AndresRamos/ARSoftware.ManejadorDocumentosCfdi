using System.Security.Cryptography.X509Certificates;
using MediatR;

namespace Core.Application.Cfdis.Queries.ObtenerCertificado
{
    public class ObtenerCertificadoQueryHandler : RequestHandler<ObtenerCertificadoQuery, X509Certificate2>
    {
        protected override X509Certificate2 Handle(ObtenerCertificadoQuery request)
        {
            return new X509Certificate2(request.Bytes,
                request.Password,
                X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);
        }
    }
}
