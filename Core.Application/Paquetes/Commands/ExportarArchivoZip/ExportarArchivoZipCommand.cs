using MediatR;

namespace Core.Application.Paquetes.Commands.ExportarArchivoZip;

public sealed record ExportarArchivoZipCommand(int PaquteId, string FileName) : IRequest;
