using System.Collections.Generic;
using Core.Application.Empresas.Models;
using MediatR;

namespace Core.Application.Empresas.Queries.BuscarEmpresas;

public class BuscarEmpresasQuery : IRequest<IEnumerable<EmpresaPerfilDto>>
{
}
