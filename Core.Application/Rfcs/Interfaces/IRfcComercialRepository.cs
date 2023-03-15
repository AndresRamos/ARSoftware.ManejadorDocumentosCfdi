using Core.Application.Rfcs.Models;

namespace Core.Application.Rfcs.Interfaces;

public interface IRfcComercialRepository
{
    Task<IEnumerable<RfcDto>> BuscarRfcsAsync();
}
