using System;
using System.IO;
using System.Threading.Tasks;
using Caliburn.Micro;
using Core.Application.Common;
using Core.Application.ConfiguracionGeneral.Commands.ActualizarCertificadoSat;
using Core.Application.ConfiguracionGeneral.Commands.ActualizarConfiguracionContpaqiComercial;
using Core.Application.ConfiguracionGeneral.Commands.ActualizarConfiguracionContpaqiContabilidad;
using Core.Application.ConfiguracionGeneral.Models;
using Core.Application.ConfiguracionGeneral.Queries.BuscarConfiguracionGeneral;
using Core.Application.Empresas.Queries.BuscarEmpresasComercial;
using Core.Application.Empresas.Queries.BuscarEmpresasContabilidad;
using MahApps.Metro.Controls.Dialogs;
using MediatR;
using Microsoft.Win32;
using Presentation.WpfApp.ViewModels.Empresas;

namespace Presentation.WpfApp.ViewModels.ConfiguracionGeneral;

public sealed class ConfiguracionGeneralViewModel : Screen
{
    private readonly ConfiguracionAplicacion _configuracionAplicacion;
    private readonly IDialogCoordinator _dialogCoordinator;
    private readonly IMediator _mediator;
    private readonly IWindowManager _windowManager;

    public ConfiguracionGeneralViewModel(IMediator mediator,
                                         IDialogCoordinator dialogCoordinator,
                                         ConfiguracionAplicacion configuracionAplicacion,
                                         IWindowManager windowManager)
    {
        _mediator = mediator;
        _dialogCoordinator = dialogCoordinator;
        _configuracionAplicacion = configuracionAplicacion;
        _windowManager = windowManager;
        DisplayName = "Configuracion General";
    }

    public ConfiguracionGeneralDto ConfiguracionGeneral { get; set; }

    public async Task InicializarAsync()
    {
        await CargarConfiguracionGeneralAsync();
    }

    private async Task CargarConfiguracionGeneralAsync()
    {
        ConfiguracionGeneral = await _mediator.Send(new BuscarConfiguracionGeneralQuery(_configuracionAplicacion.Empresa.Id));
    }

    public async Task BuscarArchivoCertificadoAsync()
    {
        try
        {
            var openFileDialog = new OpenFileDialog();
            bool? dialogResult = openFileDialog.ShowDialog();
            if (dialogResult == true)
            {
                ConfiguracionGeneral.CertificadoSat.Certificado = File.ReadAllBytes(openFileDialog.FileName);
            }
        }
        catch (Exception e)
        {
            await _dialogCoordinator.ShowMessageAsync(this, "Error", e.ToString());
        }
    }

    public async Task ActualizarConfiguracionGeneralAsync()
    {
        try
        {
            await _mediator.Send(new ActualizarCertificadoSatCommand(ConfiguracionGeneral.CertificadoSat.Certificado,
                ConfiguracionGeneral.CertificadoSat.Contrasena,
                ConfiguracionGeneral.CertificadoSat.Rfc,
                ConfiguracionGeneral.RutaDirectorioDescargas));

            await _mediator.Send(new ActualizarConfiguracionContpaqiComercialCommand(ConfiguracionGeneral.ConfiguracionContpaqiComercial));

            await _mediator.Send(
                new ActualizarConfiguracionContpaqiContabilidadCommand(ConfiguracionGeneral.ConfiguracionContpaqiContabilidad));

            await _configuracionAplicacion.CargarConfiguracionAsync();

            await TryCloseAsync();
        }
        catch (Exception e)
        {
            await _dialogCoordinator.ShowMessageAsync(this, "Error", e.ToString());
        }
    }

    public async Task BuscarEmpresaComercialAsync()
    {
        try
        {
            _configuracionAplicacion.ConfiguracionGeneral.ConfiguracionContpaqiComercial.ContpaqiSqlConnectionString =
                ConfiguracionGeneral.ConfiguracionContpaqiComercial.ContpaqiSqlConnectionString;
            var viewModel = IoC.Get<SeleccionarEmpresaContpaqiViewModel>();
            viewModel.Inicializar(await _mediator.Send(new BuscarEmpresasComercialQuery()));
            await _windowManager.ShowDialogAsync(viewModel);
            if (viewModel.SeleccionoEmpresa)
            {
                ConfiguracionGeneral.ConfiguracionContpaqiComercial.Empresa = viewModel.EmpresaSeleccionada;
            }
        }
        catch (Exception e)
        {
            await _dialogCoordinator.ShowMessageAsync(this, "Error", e.ToString());
        }
    }

    public async Task BuscarEmpresaContabilidadAsync()
    {
        try
        {
            _configuracionAplicacion.ConfiguracionGeneral.ConfiguracionContpaqiContabilidad.ContpaqiSqlConnectionString =
                ConfiguracionGeneral.ConfiguracionContpaqiContabilidad.ContpaqiSqlConnectionString;
            var viewModel = IoC.Get<SeleccionarEmpresaContpaqiViewModel>();
            viewModel.Inicializar(await _mediator.Send(new BuscarEmpresasContabilidadQuery()));
            await _windowManager.ShowDialogAsync(viewModel);
            if (viewModel.SeleccionoEmpresa)
            {
                ConfiguracionGeneral.ConfiguracionContpaqiContabilidad.Empresa = viewModel.EmpresaSeleccionada;
            }
        }
        catch (Exception e)
        {
            await _dialogCoordinator.ShowMessageAsync(this, "Error", e.ToString());
        }
    }

    public async Task Cancelar()
    {
        await TryCloseAsync();
    }
}
