using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using Core.Domain.ValueObjects;
using Infrastructure.Persistance;
using MediatR;

namespace Core.Application.ConfiguracionGeneral.Commands.ActualizarConfiguracionContpaqiContabilidad
{
    public class ActualizarConfiguracionContpaqiContabilidadCommandHandler : IRequestHandler<ActualizarConfiguracionContpaqiContabilidadCommand>
    {
        private readonly ManejadorDocumentosCfdiDbContext _context;

        public ActualizarConfiguracionContpaqiContabilidadCommandHandler(ManejadorDocumentosCfdiDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(ActualizarConfiguracionContpaqiContabilidadCommand request, CancellationToken cancellationToken)
        {
            var configuracionGeneral = await _context.ConfiguracionGeneral.FirstAsync(cancellationToken);

            configuracionGeneral.ConfiguracionContpaqiContabilidad = ConfiguracionContpaqiContabilidad.CreateInstance(
                request.ConfiguracionContpaqiContabilidad.ContpaqiSqlConnectionString,
                EmpresaContpaqi.CreateInstance(request.ConfiguracionContpaqiContabilidad.Empresa.Nombre, request.ConfiguracionContpaqiContabilidad.Empresa.BaseDatos));

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}