using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Application.Rfcs.Models;

namespace Core.Application.Rfcs.Interfaces
{
    public interface IRfcContabilidadRepository
    {
        Task<IEnumerable<RfcDto>> BuscarRfcsAsync();
    }
}