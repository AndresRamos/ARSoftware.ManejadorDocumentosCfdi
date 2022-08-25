using System.Collections.Generic;
using Core.Application.Empresas.Models;
using MediatR;

namespace Core.Application.Empresas.Queries.BuscarEmpresasComercial;

public class BuscarEmpresasComercialQuery : IRequest<IEnumerable<EmpresaContpaqiDto>>
{
}
