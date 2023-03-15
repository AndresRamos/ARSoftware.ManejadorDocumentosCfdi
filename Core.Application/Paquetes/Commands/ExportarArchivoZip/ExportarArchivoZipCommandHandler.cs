using System.Data.Entity;
using Core.Domain.Entities;
using Infrastructure.Persistance;
using MediatR;
using NLog;

namespace Core.Application.Paquetes.Commands.ExportarArchivoZip;

public class ExportarArchivoZipCommandHandler : IRequestHandler<ExportarArchivoZipCommand>
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    private readonly ManejadorDocumentosCfdiDbContext _context;

    public ExportarArchivoZipCommandHandler(ManejadorDocumentosCfdiDbContext context)
    {
        _context = context;
    }

    public async Task Handle(ExportarArchivoZipCommand request, CancellationToken cancellationToken)
    {
        Paquete paquete = await _context.Paquetes.SingleAsync(s => s.Id == request.PaquteId, cancellationToken);

        Logger.Info("Creando archivo .zip");
        using (FileStream fileStream = File.Create(request.FileName, paquete.Contenido.Length))
        {
            fileStream.Write(paquete.Contenido, 0, paquete.Contenido.Length);
        }
    }
}
