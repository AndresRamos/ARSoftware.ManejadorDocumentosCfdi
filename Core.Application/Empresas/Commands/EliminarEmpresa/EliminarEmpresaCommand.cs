using MediatR;

namespace Core.Application.Empresas.Commands.EliminarEmpresa;

public sealed record EliminarEmpresaCommand(int EmpresaId) : IRequest;
