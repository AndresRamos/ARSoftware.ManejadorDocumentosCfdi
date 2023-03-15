using System.Data.Entity;
using Core.Domain.ValueObjects;
using Infrastructure.Persistance;
using MediatR;

namespace Core.Application.ConfiguracionGeneral.Commands.ActualizarConfiguracionContpaqiContabilidad;

public class ActualizarConfiguracionContpaqiContabilidadCommandHandler : IRequestHandler<ActualizarConfiguracionContpaqiContabilidadCommand>
{
    private readonly ManejadorDocumentosCfdiDbContext _context;

    public ActualizarConfiguracionContpaqiContabilidadCommandHandler(ManejadorDocumentosCfdiDbContext context)
    {
        _context = context;
    }

    public async Task Handle(ActualizarConfiguracionContpaqiContabilidadCommand request, CancellationToken cancellationToken)
    {
        Domain.Entities.ConfiguracionGeneral configuracionGeneral = await _context.ConfiguracionGeneral.FirstAsync(cancellationToken);

        configuracionGeneral.ConfiguracionContpaqiContabilidad = ConfiguracionContpaqiContabilidad.CreateInstance(
            request.ConfiguracionContpaqiContabilidad.ContpaqiSqlConnectionString,
            EmpresaContpaqi.CreateInstance(request.ConfiguracionContpaqiContabilidad.Empresa.Nombre,
                request.ConfiguracionContpaqiContabilidad.Empresa.BaseDatos,
                request.ConfiguracionContpaqiContabilidad.Empresa.GuidAdd));

        await _context.SaveChangesAsync(cancellationToken);
    }
}
