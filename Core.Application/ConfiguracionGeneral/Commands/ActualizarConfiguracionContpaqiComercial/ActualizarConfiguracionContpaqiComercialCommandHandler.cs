using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using Core.Domain.ValueObjects;
using Infrastructure.Persistance;
using MediatR;

namespace Core.Application.ConfiguracionGeneral.Commands.ActualizarConfiguracionContpaqiComercial
{
    public class ActualizarConfiguracionContpaqiComercialCommandHandler : IRequestHandler<ActualizarConfiguracionContpaqiComercialCommand>
    {
        private readonly ManejadorDocumentosCfdiDbContext _context;

        public ActualizarConfiguracionContpaqiComercialCommandHandler(ManejadorDocumentosCfdiDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(ActualizarConfiguracionContpaqiComercialCommand request, CancellationToken cancellationToken)
        {
            Domain.Entities.ConfiguracionGeneral configuracionGeneral = await _context.ConfiguracionGeneral.FirstAsync(cancellationToken);

            configuracionGeneral.ConfiguracionContpaqiComercial = ConfiguracionContpaqiComercial.CreateInstance(
                request.ConfiguracionContpaqiComercial.ContpaqiSqlConnectionString,
                EmpresaContpaqi.CreateInstance(request.ConfiguracionContpaqiComercial.Empresa.Nombre,
                    request.ConfiguracionContpaqiComercial.Empresa.BaseDatos,
                    request.ConfiguracionContpaqiComercial.Empresa.GuidAdd));

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
