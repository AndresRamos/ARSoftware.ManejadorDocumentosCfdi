using Core.Application.Empresas.Models;
using MediatR;

namespace Core.Application.Empresas.Queries.BuscarEmpresasPermitidasPorUsuario;

public sealed record BuscarEmpresasPermitidasPorUsuarioQuery(int UsuarioId) : IRequest<IEnumerable<EmpresaPerfilDto>>;
