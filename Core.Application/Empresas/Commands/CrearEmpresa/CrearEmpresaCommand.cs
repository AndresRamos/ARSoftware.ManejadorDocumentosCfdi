using MediatR;

namespace Core.Application.Empresas.Commands.CrearEmpresa;

public sealed record CrearEmpresaCommand(string Nombre) : IRequest<int>;
