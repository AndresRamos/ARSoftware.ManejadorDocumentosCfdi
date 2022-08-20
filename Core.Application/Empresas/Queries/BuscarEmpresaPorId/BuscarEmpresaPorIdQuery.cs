using Core.Application.Empresas.Models;
using MediatR;

namespace Core.Application.Empresas.Queries.BuscarEmpresaPorId
{
    public class BuscarEmpresaPorIdQuery : IRequest<EmpresaPerfilDto>
    {
        public BuscarEmpresaPorIdQuery(int empresaId)
        {
            EmpresaId = empresaId;
        }

        public int EmpresaId { get; }
    }
}
