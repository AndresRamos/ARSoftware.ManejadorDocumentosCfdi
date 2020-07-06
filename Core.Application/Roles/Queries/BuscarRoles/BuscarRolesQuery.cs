using System.Collections.Generic;
using Core.Application.Roles.Models;
using MediatR;

namespace Core.Application.Roles.Queries.BuscarRoles
{
    public class BuscarRolesQuery : IRequest<IEnumerable<RolDto>>
    {
    }
}