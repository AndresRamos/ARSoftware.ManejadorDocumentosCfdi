﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Infrastructure.Persistance;
using MediatR;
using NLog;

namespace Core.Application.Paquetes.Commands.ExportarArchivoZip
{
    public class ExportarArchivoZipCommandHandler : IRequestHandler<ExportarArchivoZipCommand>
    {
        private readonly ManejadorDocumentosCfdiDbContext _context;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public ExportarArchivoZipCommandHandler(ManejadorDocumentosCfdiDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(ExportarArchivoZipCommand request, CancellationToken cancellationToken)
        {
            var paquete = await _context.Paquetes.SingleAsync(s => s.Id == request.PaquteId, cancellationToken);

            Logger.Info("Creando archivo .zip");
            using (var fileStream = File.Create(request.FileName, paquete.Contenido.Length))
            {
                fileStream.Write(paquete.Contenido, 0, paquete.Contenido.Length);
            }
        }
    }
}
