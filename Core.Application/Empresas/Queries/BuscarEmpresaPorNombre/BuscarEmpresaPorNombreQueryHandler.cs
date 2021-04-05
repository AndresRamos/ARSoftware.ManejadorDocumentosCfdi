using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using Core.Application.Empresas.Models;
using Infrastructure.Persistance;
using MediatR;

namespace Core.Application.Empresas.Queries.BuscarEmpresaPorNombre
{
    public class BuscarEmpresaPorNombreQueryHandler : IRequestHandler<BuscarEmpresaPorNombreQuery, EmpresaPerfilDto>
    {
        private readonly ManejadorDocumentosCfdiDbContext _context;

        public BuscarEmpresaPorNombreQueryHandler(ManejadorDocumentosCfdiDbContext context)
        {
            _context = context;
        }

        public async Task<EmpresaPerfilDto> Handle(BuscarEmpresaPorNombreQuery request, CancellationToken cancellationToken)
        {
            var empresa = await _context.Empresas.SingleOrDefaultAsync(e => e.Nombre == request.EmpresaNombre, cancellationToken);

            if (empresa is null)
            {
                return null;
            }

            return new EmpresaPerfilDto
            {
                Id = empresa.Id,
                Nombre = empresa.Nombre
            };
        }
    }
}