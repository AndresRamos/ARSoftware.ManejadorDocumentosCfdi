using System;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using Contpaqi.Sql.ADD.DocumentMetadata;
using Core.Application.Comprobantes.Interfaces;

namespace Infrastructure.ContpaqiAdd.Repositories
{
    public class ComprobanteAddRepository : IComprobanteAddComercialRepository, IComprobanteAddContabilidadRepository
    {
        private readonly AddDocumentMetadataDbContext _context;

        public ComprobanteAddRepository(AddDocumentMetadataDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExisteUuidAsync(Guid uuid, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await _context.Comprobante.AnyAsync(c => c.UUID == uuid, cancellationToken);
        }
    }
}