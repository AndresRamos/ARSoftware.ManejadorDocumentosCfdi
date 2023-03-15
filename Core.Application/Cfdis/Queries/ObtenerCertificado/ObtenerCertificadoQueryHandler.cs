using System.Security.Cryptography.X509Certificates;
using MediatR;

namespace Core.Application.Cfdis.Queries.ObtenerCertificado;

public sealed class ObtenerCertificadoQueryHandler : IRequestHandler<ObtenerCertificadoQuery, X509Certificate2>
{
    public Task<X509Certificate2> Handle(ObtenerCertificadoQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new X509Certificate2(request.Bytes,
            request.Password,
            X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable));
    }
}
