using System.Data.Entity;
using Core.Application.ConfiguracionGeneral.Models;
using Core.Domain.ValueObjects;
using Infrastructure.Persistance;
using MediatR;

namespace Core.Application.ConfiguracionGeneral.Commands.ActualizarConfiguracionGeneral;

public sealed class ActualizarConfiguracionGeneralCommandHandler : IRequestHandler<ActualizarConfiguracionGeneralCommand>
{
    private readonly ManejadorDocumentosCfdiDbContext _context;

    public ActualizarConfiguracionGeneralCommandHandler(ManejadorDocumentosCfdiDbContext context)
    {
        _context = context;
    }

    public async Task Handle(ActualizarConfiguracionGeneralCommand request, CancellationToken cancellationToken)
    {
        Domain.Entities.ConfiguracionGeneral configuracionGeneral =
            await _context.ConfiguracionGeneral.FirstAsync(c => c.Id == request.EmpresaId, cancellationToken);

        CertificadoSatDto certificadoSat = request.ConfiguracionGeneral.CertificadoSat;
        ConfiguracionContpaqiComercialDto configuracionContpaqiComercial = request.ConfiguracionGeneral.ConfiguracionContpaqiComercial;
        ConfiguracionContpaqiContabilidadDto configuracionContpaqiContabilidad =
            request.ConfiguracionGeneral.ConfiguracionContpaqiContabilidad;

        configuracionGeneral.CertificadoSat =
            CertificadoSat.CreateInstance(certificadoSat.Certificado, certificadoSat.Contrasena, certificadoSat.Rfc);

        configuracionGeneral.RutaDirectorioDescargas = request.ConfiguracionGeneral.RutaDirectorioDescargas;

        configuracionGeneral.ConfiguracionContpaqiComercial = ConfiguracionContpaqiComercial.CreateInstance(
            configuracionContpaqiComercial.ContpaqiSqlConnectionString,
            EmpresaContpaqi.CreateInstance(configuracionContpaqiComercial.Empresa.Nombre,
                configuracionContpaqiComercial.Empresa.BaseDatos,
                configuracionContpaqiComercial.Empresa.GuidAdd));

        configuracionGeneral.ConfiguracionContpaqiContabilidad = ConfiguracionContpaqiContabilidad.CreateInstance(
            configuracionContpaqiContabilidad.ContpaqiSqlConnectionString,
            EmpresaContpaqi.CreateInstance(configuracionContpaqiContabilidad.Empresa.Nombre,
                configuracionContpaqiContabilidad.Empresa.BaseDatos,
                configuracionContpaqiContabilidad.Empresa.GuidAdd));

        await _context.SaveChangesAsync(cancellationToken);
    }
}
