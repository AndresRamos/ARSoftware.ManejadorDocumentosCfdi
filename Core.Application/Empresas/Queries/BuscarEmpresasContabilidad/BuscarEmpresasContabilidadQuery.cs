using Core.Application.Empresas.Models;
using MediatR;

namespace Core.Application.Empresas.Queries.BuscarEmpresasContabilidad;

public sealed record BuscarEmpresasContabilidadQuery : IRequest<IEnumerable<EmpresaContpaqiDto>>;
