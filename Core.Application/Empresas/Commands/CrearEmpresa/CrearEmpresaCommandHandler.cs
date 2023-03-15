using Core.Domain.Entities;
using Infrastructure.Persistance;
using MediatR;

namespace Core.Application.Empresas.Commands.CrearEmpresa;

public sealed class CrearEmpresaCommandHandler : IRequestHandler<CrearEmpresaCommand, int>
{
    private readonly ManejadorDocumentosCfdiDbContext _context;

    public CrearEmpresaCommandHandler(ManejadorDocumentosCfdiDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CrearEmpresaCommand request, CancellationToken cancellationToken)
    {
        var empresa = Empresa.CreateInstance(request.Nombre);

        _context.Empresas.Add(empresa);

        await _context.SaveChangesAsync(cancellationToken);

        return empresa.Id;
    }
}
