using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Application.Empresas.Models;

namespace Core.Application.Empresas.Interfaces;

public interface IEmpresaComercialRepository
{
    Task<IEnumerable<EmpresaContpaqiDto>> BuscarEmpresasAsync();
}
