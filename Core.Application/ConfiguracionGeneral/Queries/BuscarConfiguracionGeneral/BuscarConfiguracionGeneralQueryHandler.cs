using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using Core.Application.ConfiguracionGeneral.Models;
using Infrastructure.Persistance;
using MediatR;

namespace Core.Application.ConfiguracionGeneral.Queries.BuscarConfiguracionGeneral
{
    public class BuscarConfiguracionGeneralQueryHandler : IRequestHandler<BuscarConfiguracionGeneralQuery, ConfiguracionGeneralDto>
    {
        private readonly ManejadorDocumentosCfdiDbContext _context;

        public BuscarConfiguracionGeneralQueryHandler(ManejadorDocumentosCfdiDbContext context)
        {
            _context = context;
        }

        public async Task<ConfiguracionGeneralDto> Handle(BuscarConfiguracionGeneralQuery request, CancellationToken cancellationToken)
        {
            var configuracionGeneral = await _context.ConfiguracionGeneral.FirstAsync(cancellationToken);

            return new ConfiguracionGeneralDto
            {
                Id = configuracionGeneral.Id,
                CertificadoSat = new CertificadoSatDto
                {
                    Certificado = configuracionGeneral.CertificadoSat.Certificado,
                    Contrasena = configuracionGeneral.CertificadoSat.Contrasena,
                    Rfc = configuracionGeneral.CertificadoSat.Rfc
                },
                RutaDirectorioDescargas = configuracionGeneral.RutaDirectorioDescargas
            };
        }
    }
}