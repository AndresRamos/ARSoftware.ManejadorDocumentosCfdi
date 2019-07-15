using System;
using System.Data.Entity;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Persistance;
using Infrastructure.Sat;
using Infrastructure.Sat.Services;
using MediatR;

namespace Core.Application.Solicitudes.Commands.DescargarSolicitud
{
    public class DescargarSolicitudCommandHandler : IRequestHandler<DescargarSolicitudCommand, Unit>
    {
        private readonly ManejadorDocumentosCfdiDbContext _context;

        public DescargarSolicitudCommandHandler(ManejadorDocumentosCfdiDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DescargarSolicitudCommand request, CancellationToken cancellationToken)
        {
            var configuracionGeneral = await _context.ConfiguracionGeneral.FirstAsync(cancellationToken);
            var solicitud = await _context.Solicitudes.SingleAsync(s => s.Id == request.SolicitudId, cancellationToken);

            var certificadoSat = CertificadoService.ObtenerCertificado(configuracionGeneral.CertificadoSat.Certificado, configuracionGeneral.CertificadoSat.Contrasena);

            var descargarSolicitud = new DescargarSolicitudService(UrlsSat.UrlDescargarSolicitud, UrlsSat.UrlDescargarSolicitudAction);
            var xmlDescarga = descargarSolicitud.Generate(certificadoSat, configuracionGeneral.CertificadoSat.RfcEmisor, solicitud.PaqueteId);
            var descargaResponse = descargarSolicitud.Send(solicitud.Autorizacion);

            var path = "./Paquetes/";
            var file = Convert.FromBase64String(descargaResponse);
            Directory.CreateDirectory(path);

            using (var fs = File.Create(path + solicitud.PaqueteId + ".zip", file.Length))
            {
                fs.Write(file, 0, file.Length);
            }

            return Unit.Value;
        }
    }
}