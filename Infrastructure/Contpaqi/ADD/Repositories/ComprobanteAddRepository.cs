using ARSoftware.Contpaqi.Add.Sql.Contexts;
using Core.Application.Comprobantes.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Contpaqi.ADD.Repositories;

public class ComprobanteAddRepository : IComprobanteAddComercialRepository, IComprobanteAddContabilidadRepository
{
    private readonly ContpaqiAddDocumentMetadataDbContext _context;

    public ComprobanteAddRepository(ContpaqiAddDocumentMetadataDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ExisteUuidAsync(Guid uuid, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await _context.Comprobante.AnyAsync(c => c.UUID == uuid, cancellationToken);
    }
}
