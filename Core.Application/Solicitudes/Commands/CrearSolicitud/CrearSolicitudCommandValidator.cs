using System.Data.Entity;
using Core.Domain.Entities;
using FluentValidation;
using Infrastructure.Persistance;

namespace Core.Application.Solicitudes.Commands.CrearSolicitud;

public sealed class CrearSolicitudCommandValidator : AbstractValidator<CrearSolicitudCommand>
{
    private readonly ManejadorDocumentosCfdiDbContext _context;

    public CrearSolicitudCommandValidator(ManejadorDocumentosCfdiDbContext context)
    {
        _context = context;
        RuleFor(c => c.EmpresaId).MustAsync(ExisteEmpresaAsync).WithMessage("El id de la empresa no es un id valido.");
    }

    public async Task<bool> ExisteEmpresaAsync(int empresaId, CancellationToken cancellationToken)
    {
        return await _context.Empresas.SingleOrDefaultAsync(e => e.Id == empresaId, cancellationToken) != null;
    }

    public async Task<bool> ExisteUsuarioAsync(int usuarioId, CancellationToken cancellationToken)
    {
        return await _context.Usuarios.SingleOrDefaultAsync(u => u.Id == usuarioId, cancellationToken) != null;
    }

    public async Task<bool> UsuarioTieneEmpresaPermitida(int usuarioId, int empresaId, CancellationToken cancellationToken)
    {
        Usuario usuario = await _context.Usuarios.FirstAsync(u => u.Id == usuarioId, cancellationToken);
        Empresa empresa = await _context.Empresas.FirstAsync(e => e.Id == empresaId, cancellationToken);

        return usuario.EmpresasPermitidas.Contains(empresa);
    }
}
