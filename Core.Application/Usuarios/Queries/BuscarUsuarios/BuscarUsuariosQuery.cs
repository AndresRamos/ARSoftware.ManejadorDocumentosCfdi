using System.Collections.Generic;
using Core.Application.Usuarios.Models;
using MediatR;

namespace Core.Application.Usuarios.Queries.BuscarUsuarios
{
    public class BuscarUsuariosQuery : IRequest<IEnumerable<UsuarioDto>>
    {
    }
}
