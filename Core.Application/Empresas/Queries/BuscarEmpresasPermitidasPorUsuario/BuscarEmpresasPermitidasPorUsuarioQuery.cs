using System.Collections.Generic;
using Core.Application.Empresas.Models;
using MediatR;

namespace Core.Application.Empresas.Queries.BuscarEmpresasPermitidasPorUsuario
{
    public class BuscarEmpresasPermitidasPorUsuarioQuery : IRequest<IEnumerable<EmpresaPerfilDto>>
    {
        public BuscarEmpresasPermitidasPorUsuarioQuery(int usuarioId)
        {
            UsuarioId = usuarioId;
        }

        public int UsuarioId { get; }
    }
}
