using MediatR;

namespace Core.Application.ConfiguracionGeneral.Commands.ActualizarCertificadoSat
{
    public class ActualizarCertificadoSatCommand : IRequest
    {
        public ActualizarCertificadoSatCommand(byte[] certificadoSat, string contrasena, string rfcEmisor)
        {
            CertificadoSat = certificadoSat;
            Contrasena = contrasena;
            RfcEmisor = rfcEmisor;
        }

        public byte[] CertificadoSat { get; }

        public string Contrasena { get; }

        public string RfcEmisor { get; }
    }
}