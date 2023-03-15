using MediatR;

namespace Core.Application.Usuarios.Commands.AgregarEmpresaPermitida;

public sealed record AgregarEmpresaPermitidaCommand(int UsuarioId, int EmpresaId) : IRequest;
