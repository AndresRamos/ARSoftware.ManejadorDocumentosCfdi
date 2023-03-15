using Caliburn.Micro;
using Core.Application.Paquetes.Commands.ExportarArchivoZip;
using Core.Application.Paquetes.Models;
using MahApps.Metro.Controls.Dialogs;
using MediatR;
using Microsoft.Win32;

namespace Presentation.WpfApp.ViewModels.Solicitudes;

public sealed class SolicitudPaquetesViewModel : Screen
{
    private readonly IDialogCoordinator _dialogCoordinator;
    private readonly IMediator _mediator;
    private PaqueteDto _paqueteSeleccionado;

    public SolicitudPaquetesViewModel(IMediator mediator, IDialogCoordinator dialogCoordinator)
    {
        _mediator = mediator;
        _dialogCoordinator = dialogCoordinator;
        DisplayName = "Paquetes";
    }

    public BindableCollection<PaqueteDto> Paquetes { get; } = new();

    public PaqueteDto PaqueteSeleccionado
    {
        get => _paqueteSeleccionado;
        set
        {
            if (Equals(value, _paqueteSeleccionado))
                return;

            _paqueteSeleccionado = value;
            NotifyOfPropertyChange(() => PaqueteSeleccionado);
            RaiseGuards();
        }
    }

    public bool CanGuardarPaqueteAsync => PaqueteSeleccionado != null;

    public void Inicializar(IEnumerable<PaqueteDto> paquetes)
    {
        Paquetes.Clear();
        Paquetes.AddRange(paquetes);
    }

    public async Task GuardarPaqueteAsync()
    {
        try
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "ZIP (.zip)|*.zip";
            saveFileDialog.FileName = $"{PaqueteSeleccionado.IdSat}.zip";
            if (saveFileDialog.ShowDialog() == true)
                await _mediator.Send(new ExportarArchivoZipCommand(PaqueteSeleccionado.Id, saveFileDialog.FileName));
        }
        catch (Exception e)
        {
            await _dialogCoordinator.ShowMessageAsync(this, "Error", e.ToString());
        }
    }

    private void RaiseGuards()
    {
        NotifyOfPropertyChange(() => CanGuardarPaqueteAsync);
    }
}
