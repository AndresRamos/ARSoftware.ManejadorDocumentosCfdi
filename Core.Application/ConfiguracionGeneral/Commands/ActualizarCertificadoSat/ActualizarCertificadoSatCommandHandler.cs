using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using Core.Domain.ValueObjects;
using Infrastructure.Persistance;
using MediatR;

namespace Core.Application.ConfiguracionGeneral.Commands.ActualizarCertificadoSat;

public class ActualizarCertificadoSatCommandHandler : IRequestHandler<ActualizarCertificadoSatCommand, Unit>
{
    private readonly ManejadorDocumentosCfdiDbContext _context;

    public ActualizarCertificadoSatCommandHandler(ManejadorDocumentosCfdiDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(ActualizarCertificadoSatCommand request, CancellationToken cancellationToken)
    {
        Domain.Entities.ConfiguracionGeneral configuracionGeneral = await _context.ConfiguracionGeneral.FirstAsync(cancellationToken);

        configuracionGeneral.CertificadoSat = CertificadoSat.CreateInstance(request.CertificadoSat, request.Contrasena, request.RfcEmisor);

        configuracionGeneral.RutaDirectorioDescargas = request.RutaDirectorioDescargas;

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
