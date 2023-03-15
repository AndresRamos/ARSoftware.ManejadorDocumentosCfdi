using System.Security.Cryptography.X509Certificates;
using MediatR;

namespace Core.Application.Cfdis.Queries.ObtenerCertificado;

public sealed record ObtenerCertificadoQuery(byte[] Bytes, string Password) : IRequest<X509Certificate2>;
