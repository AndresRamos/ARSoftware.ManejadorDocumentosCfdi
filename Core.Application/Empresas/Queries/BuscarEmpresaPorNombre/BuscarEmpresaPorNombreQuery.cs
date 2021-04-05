using Core.Application.Empresas.Models;
using MediatR;

namespace Core.Application.Empresas.Queries.BuscarEmpresaPorNombre
{
    public class BuscarEmpresaPorNombreQuery : IRequest<EmpresaPerfilDto>
    {
        public BuscarEmpresaPorNombreQuery(string empresaNombre)
        {
            EmpresaNombre = empresaNombre;
        }

        public string EmpresaNombre { get; }
    }
}