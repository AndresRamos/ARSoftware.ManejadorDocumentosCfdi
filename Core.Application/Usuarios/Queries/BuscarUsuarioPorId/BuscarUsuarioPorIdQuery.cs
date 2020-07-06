using Core.Application.Usuarios.Models;
using MediatR;

namespace Core.Application.Usuarios.Queries.BuscarUsuarioPorId
{
    public class BuscarUsuarioPorIdQuery : IRequest<UsuarioDto>
    {
        public BuscarUsuarioPorIdQuery(int usuarioId)
        {
            UsuarioId = usuarioId;
        }

        public int UsuarioId { get; }
    }
}