// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace Core.Domain.Entities
{
    public abstract class SolicitudWebBase
    {
        protected SolicitudWebBase()
        {
        }

        protected SolicitudWebBase(string solicitud, string respuesta)
        {
            Solicitud = solicitud;
            Respuesta = respuesta;
        }

        public int Id { get; private set; }
        public string Solicitud { get; private set; }
        public string Respuesta { get; private set; }
    }
}
