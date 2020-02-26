namespace Infrastructure.Sat.Models
{
    public class SolicitudResult
    {
        public string Token { get; set; }
        public string CodEstatus { get; set; }
        public string IdSolicitud { get; set; }
        public string Mensaje { get; set; }
        public string CodigoEstadoSolicitud { get; set; }
        public string EstadoSolicitud { get; set; }
        public string IdsPaquetes { get; set; }
        public string Paquete { get; set; }
        public string NumeroCfdis { get; set; }
        public string WebResponse { get; set; }

        public static SolicitudResult CrearAutenticaResult(string token, string webResponse)
        {
            return new SolicitudResult
            {
                Token = token,
                WebResponse = webResponse
            };
        }

        public static SolicitudResult CrearGenerarSolicitudResult(string codEstatus, string idSolicitud, string mensaje, string webResponse)
        {
            return new SolicitudResult
            {
                CodEstatus = codEstatus,
                IdSolicitud = idSolicitud,
                Mensaje = mensaje,
                WebResponse = webResponse
            };
        }

        public static SolicitudResult CrearVerificarSolicitudResult(string codEstatus, string codigoEstadoSolicitud, string estadoSolicitud, string numeroCfdis, string mensaje, string idsPaquetes, string webResponse)
        {
            return new SolicitudResult
            {
                CodEstatus = codEstatus,
                CodigoEstadoSolicitud = codigoEstadoSolicitud,
                EstadoSolicitud = estadoSolicitud,
                NumeroCfdis = numeroCfdis,
                Mensaje = mensaje,
                IdsPaquetes = idsPaquetes,
                WebResponse = webResponse
            };
        }

        public static SolicitudResult CrearDescargaSolicitudResult(string codEstatus, string mensaje, string paquete, string webResponse)
        {
            return new SolicitudResult
            {
                CodEstatus = codEstatus,
                Mensaje = mensaje,
                Paquete = paquete,
                WebResponse = webResponse
            };
        }

        public (string token, string response) GetAutenticaResult()
        {
            return (Token, WebResponse);
        }

        public (string codEstatus, string idSolicitud, string mensaje, string response) GetGenerarSolicitudResult()
        {
            return (CodEstatus, IdSolicitud, Mensaje, WebResponse);
        }

        public (string codEstatus, string codigoEstadoSolicitud, string estadoSolicitud, string numeroCfdis, string mensaje, string idsPaquetes, string response) GetVerificarSolicitudResult()
        {
            return (CodEstatus, CodigoEstadoSolicitud, EstadoSolicitud, NumeroCfdis, Mensaje, IdsPaquetes, WebResponse);
        }

        public (string codEstatus, string mensaje, string paquete, string response) GetDescargaSolicitudResult()
        {
            return (CodEstatus, Mensaje, Paquete, WebResponse);
        }
    }
}