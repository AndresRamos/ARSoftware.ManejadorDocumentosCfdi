using System.Collections.Generic;
using Core.Application.Empresas.Models;
using MediatR;

namespace Core.Application.Empresas.Queries.BuscarEmpresasContabilidad
{
    public class BuscarEmpresasContabilidadQuery : IRequest<IEnumerable<EmpresaContpaqiDto>>
    {
    }
}
