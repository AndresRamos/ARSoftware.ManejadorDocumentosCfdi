using System.IO.Compression;
using MediatR;

namespace Core.Application.Cfdis.ExtraerCfdi;

public class ExtraerCfdiCommandHandler : IRequestHandler<ExtraerCfdiCommand>
{
    public Task Handle(ExtraerCfdiCommand request, CancellationToken cancellationToken)
    {
        ZipFile.ExtractToDirectory(
            @"\\AR-SERVER\AR Software\Desarrollos\ManejadorDocumentosCfdi\SDN2CECD9AC-3A76-448C-8452-D46DC9BAA909_01.zip",
            @"\\AR-SERVER\AR Software\Desarrollos\ManejadorDocumentosCfdi\Test");
        return Task.CompletedTask;
    }
}
