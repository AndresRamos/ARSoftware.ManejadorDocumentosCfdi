using System.Data.Entity;
using Core.Application.ConfiguracionGeneral.Models;
using Core.Application.Empresas.Models;
using Infrastructure.Persistance;
using MediatR;

namespace Core.Application.ConfiguracionGeneral.Queries.BuscarConfiguracionGeneral;

public class BuscarConfiguracionGeneralQueryHandler : IRequestHandler<BuscarConfiguracionGeneralQuery, ConfiguracionGeneralDto>
{
    private readonly ManejadorDocumentosCfdiDbContext _context;

    public BuscarConfiguracionGeneralQueryHandler(ManejadorDocumentosCfdiDbContext context)
    {
        _context = context;
    }

    public async Task<ConfiguracionGeneralDto> Handle(BuscarConfiguracionGeneralQuery request, CancellationToken cancellationToken)
    {
        Domain.Entities.ConfiguracionGeneral configuracionGeneral =
            await _context.ConfiguracionGeneral.SingleAsync(c => c.Id == request.EmpresaId, cancellationToken);

        return new ConfiguracionGeneralDto(configuracionGeneral.Id,
            new CertificadoSatDto(configuracionGeneral.CertificadoSat.Certificado,
                configuracionGeneral.CertificadoSat.Contrasena,
                configuracionGeneral.CertificadoSat.Rfc),
            configuracionGeneral.RutaDirectorioDescargas,
            new ConfiguracionContpaqiComercialDto(configuracionGeneral.ConfiguracionContpaqiComercial.ContpaqiSqlConnectionString,
                new EmpresaContpaqiDto(configuracionGeneral.ConfiguracionContpaqiComercial.Empresa.Nombre,
                    configuracionGeneral.ConfiguracionContpaqiComercial.Empresa.BaseDatos,
                    configuracionGeneral.ConfiguracionContpaqiComercial.Empresa.GuidAdd)),
            new ConfiguracionContpaqiContabilidadDto(configuracionGeneral.ConfiguracionContpaqiContabilidad.ContpaqiSqlConnectionString,
                new EmpresaContpaqiDto(configuracionGeneral.ConfiguracionContpaqiContabilidad.Empresa.Nombre,
                    configuracionGeneral.ConfiguracionContpaqiContabilidad.Empresa.BaseDatos,
                    configuracionGeneral.ConfiguracionContpaqiContabilidad.Empresa.GuidAdd)));
    }
}
