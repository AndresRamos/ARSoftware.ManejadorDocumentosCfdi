using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Data;
using Caliburn.Micro;
using Core.Application.Cfdis.Models;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using OfficeOpenXml;

namespace Presentation.WpfApp.ViewModels.Cfdis;

public sealed class ListaCfdisViewModel : Screen
{
    private readonly IDialogCoordinator _dialogCoordinator;
    private CfdiEncabezadoDto _comprobanteSeleccionado;
    private string _filtro;

    public ListaCfdisViewModel(IDialogCoordinator dialogCoordinator)
    {
        _dialogCoordinator = dialogCoordinator;
        DisplayName = "CFDIs";
        ComprobantesView = CollectionViewSource.GetDefaultView(Comprobantes);
        ComprobantesView.Filter = ComprobantesView_Filter;
    }

    public string Filtro
    {
        get => _filtro;
        set
        {
            if (value == _filtro)
                return;

            _filtro = value;
            NotifyOfPropertyChange(() => Filtro);
            ComprobantesView.Refresh();
            RaiseGuards();
        }
    }

    public BindableCollection<CfdiEncabezadoDto> Comprobantes { get; } = new();

    public ICollectionView ComprobantesView { get; }

    public CfdiEncabezadoDto ComprobanteSeleccionado
    {
        get => _comprobanteSeleccionado;
        set
        {
            if (value == _comprobanteSeleccionado)
                return;

            _comprobanteSeleccionado = value;
            NotifyOfPropertyChange(() => ComprobanteSeleccionado);
        }
    }

    public bool CanExportarExcelAsync => ComprobantesView.Cast<object>().Any();

    public void Inicializar(IEnumerable<CfdiEncabezadoDto> comprobantes)
    {
        Comprobantes.Clear();
        Comprobantes.AddRange(comprobantes);
        RaiseGuards();
    }

    public async Task ExportarExcelAsync()
    {
        var saveFileDialog = new SaveFileDialog { Filter = "Excel | *.xlsx", FileName = "CFDIs.xlsx" };
        if (saveFileDialog.ShowDialog() != true)
            return;

        ProgressDialogController progressDialogController = await _dialogCoordinator.ShowProgressAsync(this, "Exportando", "Exportando");
        progressDialogController.SetIndeterminate();
        await Task.Delay(1000);

        try
        {
            using (var excelPackage = new ExcelPackage(new FileInfo(saveFileDialog.FileName)))
            {
                ExcelWorksheet excelWorksheet = excelPackage.Workbook.Worksheets.Add("CFDIs");
                excelWorksheet.Cells.LoadFromCollection(ComprobantesView.Cast<CfdiEncabezadoDto>(), true);
                excelWorksheet.Cells.AutoFitColumns();
                excelPackage.Save();
            }

            Process.Start(new ProcessStartInfo(saveFileDialog.FileName) { UseShellExecute = true });
        }
        catch (Exception e)
        {
            await _dialogCoordinator.ShowMessageAsync(this, "Error", e.ToString());
        }
        finally
        {
            await progressDialogController.CloseAsync();
        }
    }

    private void RaiseGuards()
    {
        NotifyOfPropertyChange(() => CanExportarExcelAsync);
    }

    private bool ComprobantesView_Filter(object obj)
    {
        if (obj is null)
            throw new ArgumentNullException(nameof(obj));

        if (!(obj is CfdiEncabezadoDto comprobante))
            throw new ArgumentNullException(nameof(comprobante));

        return string.IsNullOrWhiteSpace(Filtro) ||
               comprobante.ComprobanteFecha?.IndexOf(Filtro, StringComparison.OrdinalIgnoreCase) >= 0 ||
               comprobante.ComprobanteTipo?.IndexOf(Filtro, StringComparison.OrdinalIgnoreCase) >= 0 ||
               comprobante.ComprobanteSerie?.IndexOf(Filtro, StringComparison.OrdinalIgnoreCase) >= 0 ||
               comprobante.ComprobanteFolio?.IndexOf(Filtro, StringComparison.OrdinalIgnoreCase) >= 0 ||
               comprobante.ComprobanteMoneda?.IndexOf(Filtro, StringComparison.OrdinalIgnoreCase) >= 0 ||
               comprobante.ComprobanteTotal?.IndexOf(Filtro, StringComparison.OrdinalIgnoreCase) >= 0 ||
               comprobante.EmisorNombre?.IndexOf(Filtro, StringComparison.OrdinalIgnoreCase) >= 0 ||
               comprobante.EmisorRfc?.IndexOf(Filtro, StringComparison.OrdinalIgnoreCase) >= 0 ||
               comprobante.ReceptorNombre?.IndexOf(Filtro, StringComparison.OrdinalIgnoreCase) >= 0 ||
               comprobante.ReceptorRfc?.IndexOf(Filtro, StringComparison.OrdinalIgnoreCase) >= 0 ||
               comprobante.Uuid?.IndexOf(Filtro, StringComparison.OrdinalIgnoreCase) >= 0;
    }
}
