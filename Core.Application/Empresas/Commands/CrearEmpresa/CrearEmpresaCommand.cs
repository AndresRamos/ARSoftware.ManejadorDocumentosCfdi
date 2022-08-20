using MediatR;

namespace Core.Application.Empresas.Commands.CrearEmpresa
{
    public class CrearEmpresaCommand : IRequest<int>
    {
        public CrearEmpresaCommand(string nombre)
        {
            Nombre = nombre;
        }

        public string Nombre { get; }
    }
}
