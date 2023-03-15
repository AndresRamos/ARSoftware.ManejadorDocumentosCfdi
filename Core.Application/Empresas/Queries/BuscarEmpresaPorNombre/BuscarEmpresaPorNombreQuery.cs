using Core.Application.Empresas.Models;
using MediatR;

namespace Core.Application.Empresas.Queries.BuscarEmpresaPorNombre;

public sealed record BuscarEmpresaPorNombreQuery(string EmpresaNombre) : IRequest<EmpresaPerfilDto>;
