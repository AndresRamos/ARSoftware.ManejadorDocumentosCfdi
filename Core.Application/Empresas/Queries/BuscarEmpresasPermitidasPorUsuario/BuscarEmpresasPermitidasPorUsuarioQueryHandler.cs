using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Application.Empresas.Models;
using Core.Domain.Entities;
using Infrastructure.Persistance;
using MediatR;

namespace Core.Application.Empresas.Queries.BuscarEmpresasPermitidasPorUsuario;

public class BuscarEmpresasPermitidasPorUsuarioQueryHandler : IRequestHandler<BuscarEmpresasPermitidasPorUsuarioQuery,
    IEnumerable<EmpresaPerfilDto>>
{
    private readonly ManejadorDocumentosCfdiDbContext _context;

    public BuscarEmpresasPermitidasPorUsuarioQueryHandler(ManejadorDocumentosCfdiDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<EmpresaPerfilDto>> Handle(BuscarEmpresasPermitidasPorUsuarioQuery query,
                                                            CancellationToken cancellationToken)
    {
        Usuario usuario = await _context.Usuarios.Include(u => u.EmpresasPermitidas)
            .SingleOrDefaultAsync(u => u.Id == query.UsuarioId, cancellationToken);

        if (usuario == null)
        {
            throw new ObjectNotFoundException($"No se encontro el usuario con id {query.UsuarioId}.");
        }

        return usuario.EmpresasPermitidas.Select(e => new EmpresaPerfilDto { Id = e.Id, Nombre = e.Nombre }).ToList();
    }
}
