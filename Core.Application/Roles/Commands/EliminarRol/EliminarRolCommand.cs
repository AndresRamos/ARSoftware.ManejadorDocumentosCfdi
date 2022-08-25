using MediatR;

namespace Core.Application.Roles.Commands.EliminarRol;

public class EliminarRolCommand : IRequest
{
    public EliminarRolCommand(int rolId)
    {
        RolId = rolId;
    }

    public int RolId { get; }
}
