using Core.Application.Roles.Models;
using MediatR;

namespace Core.Application.Roles.Queries.BuscarRolPorId;

public class BuscarRolPorIdQuery : IRequest<RolDto>
{
    public BuscarRolPorIdQuery(int rolId)
    {
        RolId = rolId;
    }

    public int RolId { get; }
}
