using MediatR;

namespace Core.Application.Empresas.Commands.ActualizarEmpresaPerfil
{
    public class ActualizarEmpresaPerfilCommand : IRequest
    {
        public ActualizarEmpresaPerfilCommand(int empresaId, string nombre)
        {
            EmpresaId = empresaId;
            Nombre = nombre;
        }

        public int EmpresaId { get; }
        public string Nombre { get; }
    }
}