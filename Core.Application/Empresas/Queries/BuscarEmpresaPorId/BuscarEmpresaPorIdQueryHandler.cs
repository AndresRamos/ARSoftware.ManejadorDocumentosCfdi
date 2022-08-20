using System.Data.Entity;
using System.Data.Entity.Core;
using System.Threading;
using System.Threading.Tasks;
using Core.Application.Empresas.Models;
using Core.Domain.Entities;
using Infrastructure.Persistance;
using MediatR;

namespace Core.Application.Empresas.Queries.BuscarEmpresaPorId
{
    public class BuscarEmpresaPorIdQueryHandler : IRequestHandler<BuscarEmpresaPorIdQuery, EmpresaPerfilDto>
    {
        private readonly ManejadorDocumentosCfdiDbContext _context;

        public BuscarEmpresaPorIdQueryHandler(ManejadorDocumentosCfdiDbContext context)
        {
            _context = context;
        }

        public async Task<EmpresaPerfilDto> Handle(BuscarEmpresaPorIdQuery request, CancellationToken cancellationToken)
        {
            Empresa empresa = await _context.Empresas.SingleOrDefaultAsync(e => e.Id == request.EmpresaId, cancellationToken);

            if (empresa is null)
            {
                throw new ObjectNotFoundException($"No se encontro la empresa con id {request.EmpresaId}.");
            }

            return new EmpresaPerfilDto { Id = empresa.Id, Nombre = empresa.Nombre };
        }
    }
}
