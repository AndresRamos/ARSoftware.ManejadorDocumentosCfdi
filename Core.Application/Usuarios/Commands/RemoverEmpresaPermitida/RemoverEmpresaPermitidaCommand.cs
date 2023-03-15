using MediatR;

namespace Core.Application.Usuarios.Commands.RemoverEmpresaPermitida;

public sealed record RemoverEmpresaPermitidaCommand(int UsuarioId, int EmpresaId) : IRequest;
