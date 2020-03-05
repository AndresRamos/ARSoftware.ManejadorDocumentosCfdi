using MediatR;

namespace Core.Application.Paquetes.Commands.ExportarArchivoZip
{
    public class ExportarArchivoZipCommand : IRequest
    {
        public ExportarArchivoZipCommand(int paquteId, string fileName)
        {
            PaquteId = paquteId;
            FileName = fileName;
        }

        public int PaquteId { get; }
        public string FileName { get;  }
    }
}