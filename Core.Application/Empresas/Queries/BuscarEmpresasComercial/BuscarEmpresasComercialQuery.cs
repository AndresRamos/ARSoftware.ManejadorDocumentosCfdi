using Core.Application.Empresas.Models;
using MediatR;

namespace Core.Application.Empresas.Queries.BuscarEmpresasComercial;

public sealed record BuscarEmpresasComercialQuery : IRequest<IEnumerable<EmpresaContpaqiDto>>;
