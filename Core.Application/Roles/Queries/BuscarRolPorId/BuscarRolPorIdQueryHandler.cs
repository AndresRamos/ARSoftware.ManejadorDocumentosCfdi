using System.Data.Entity;
using System.Data.Entity.Core;
using System.Threading;
using System.Threading.Tasks;
using Common.Models;
using Core.Application.Roles.Models;
using Core.Domain.Entities;
using Infrastructure.Persistance;
using MediatR;

namespace Core.Application.Roles.Queries.BuscarRolPorId
{
    public class BuscarRolPorIdQueryHandler : IRequestHandler<BuscarRolPorIdQuery, RolDto>
    {
        private readonly ManejadorDocumentosCfdiDbContext _context;

        public BuscarRolPorIdQueryHandler(ManejadorDocumentosCfdiDbContext context)
        {
            _context = context;
        }

        public async Task<RolDto> Handle(BuscarRolPorIdQuery request, CancellationToken cancellationToken)
        {
            Rol rol = await _context.Roles.SingleOrDefaultAsync(r => r.Id == request.RolId, cancellationToken);

            if (rol == null)
            {
                throw new ObjectNotFoundException($"No se encontro el rol con el id {request.RolId}");
            }

            return new RolDto(rol.Id, rol.Nombre, rol.Descripcion, rol.Permisos.UnpackToPermisosDto());
        }
    }
}
