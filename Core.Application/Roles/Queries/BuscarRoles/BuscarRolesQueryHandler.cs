using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.Models;
using Core.Application.Roles.Models;
using Infrastructure.Persistance;
using MediatR;

namespace Core.Application.Roles.Queries.BuscarRoles
{
    public class BuscarRolesQueryHandler : IRequestHandler<BuscarRolesQuery, IEnumerable<RolDto>>
    {
        private readonly ManejadorDocumentosCfdiDbContext _context;

        public BuscarRolesQueryHandler(ManejadorDocumentosCfdiDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RolDto>> Handle(BuscarRolesQuery request, CancellationToken cancellationToken)
        {
            return (await _context.Roles.ToListAsync(cancellationToken))
                .Select(r => new RolDto(r.Id, r.Nombre, r.Descripcion, r.Permisos.UnpackToPermisosDto()))
                .ToList();
        }
    }
}
