using System;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Application.Comprobantes.Interfaces
{
    public interface IComprobanteAddRepository
    {
        Task<bool> ExisteUuidAsync(Guid uuid, CancellationToken cancellationToken);
    }
}
