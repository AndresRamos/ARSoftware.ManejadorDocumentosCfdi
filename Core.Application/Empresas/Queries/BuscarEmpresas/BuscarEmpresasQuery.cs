using Core.Application.Empresas.Models;
using MediatR;

namespace Core.Application.Empresas.Queries.BuscarEmpresas;

public sealed record BuscarEmpresasQuery : IRequest<IEnumerable<EmpresaPerfilDto>>;
