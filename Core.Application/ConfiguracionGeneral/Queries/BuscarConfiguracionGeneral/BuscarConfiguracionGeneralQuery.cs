using Core.Application.ConfiguracionGeneral.Models;
using MediatR;

namespace Core.Application.ConfiguracionGeneral.Queries.BuscarConfiguracionGeneral
{
    public class BuscarConfiguracionGeneralQuery : IRequest<ConfiguracionGeneralDto>
    {
        public BuscarConfiguracionGeneralQuery(int empresaId)
        {
            EmpresaId = empresaId;
        }

        public int EmpresaId { get; }
    }
}
