using MediatR;

namespace Core.Application.Empresas.Commands.ActualizarEmpresaPerfil;

public sealed record ActualizarEmpresaPerfilCommand(int EmpresaId, string Nombre) : IRequest;
