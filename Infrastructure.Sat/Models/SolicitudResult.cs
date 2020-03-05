using System;
using System.Collections.Generic;

namespace Infrastructure.Sat.Models
{
    public class SolicitudResult
    {
        private string Token { get; set; }
        private string CodEstatus { get; set; }
        private string IdSolicitud { get; set; }
        private string Mensaje { get; set; }
        private string CodigoEstadoSolicitud { get; set; }
        private string EstadoSolicitud { get; set; }
        private List<string> IdsPaquetes { get; set; }
        private string Paquete { get; set; }
        private string NumeroCfdis { get; set; }
        private string WebResponse { get; set; }
        private string FaultCode { get; set; }
        private string FaultString { get; set; }
        public Exception Exception { get; set; }

        public static SolicitudResult CrearAutenticaResult(string token, string faultCode, string faultString, string webResponse)
        {
            return new SolicitudResult
            {
                Token = token,
                FaultCode = faultCode,
                FaultString = faultString,
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

        public static SolicitudResult CrearVerificarSolicitudResult(string codEstatus, string codigoEstadoSolicitud, string estadoSolicitud, string numeroCfdis, string mensaje, List<string> idsPaquetes, string webResponse)
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

        public (string token, string faultCode, string faultString, string response) GetAutenticaResult()
        {
            return (Token, FaultCode, FaultString, WebResponse);
        }

        public (string codEstatus, string idSolicitud, string mensaje, string response) GetGenerarSolicitudResult()
        {
            return (CodEstatus, IdSolicitud, Mensaje, WebResponse);
        }

        public (string codEstatus, string codigoEstadoSolicitud, string estadoSolicitud, string numeroCfdis, string mensaje, List<string> idsPaquetes, string response) GetVerificarSolicitudResult()
        {
            return (CodEstatus, CodigoEstadoSolicitud, EstadoSolicitud, NumeroCfdis, Mensaje, IdsPaquetes, WebResponse);
        }

        public (string codEstatus, string mensaje, string paquete, string response) GetDescargaSolicitudResult()
        {
            return (CodEstatus, Mensaje, Paquete, WebResponse);
        }
    }
}