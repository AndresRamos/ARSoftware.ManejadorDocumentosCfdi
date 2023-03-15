using Core.Application.Empresas.Models;
using MediatR;

namespace Core.Application.Empresas.Queries.BuscarEmpresaPorId;

public sealed record BuscarEmpresaPorIdQuery(int EmpresaId) : IRequest<EmpresaPerfilDto>;
