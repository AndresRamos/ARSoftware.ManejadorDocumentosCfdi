using MediatR;

namespace Core.Application.ConfiguracionGeneral.Commands.ActualizarCertificadoSat
{
    public class ActualizarCertificadoSatCommand : IRequest
    {
        public ActualizarCertificadoSatCommand(byte[] certificadoSat, string contrasena, string rfcEmisor, string rutaDirectorioDescargas)
        {
            CertificadoSat = certificadoSat;
            Contrasena = contrasena;
            RfcEmisor = rfcEmisor;
            RutaDirectorioDescargas = rutaDirectorioDescargas;
        }

        public byte[] CertificadoSat { get; }

        public string Contrasena { get; }

        public string RfcEmisor { get; }

        public string RutaDirectorioDescargas { get;  }
    }
}