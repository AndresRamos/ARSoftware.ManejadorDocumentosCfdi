﻿using System;
using System.IO;
using System.Threading.Tasks;
using Caliburn.Micro;
using Core.Application.ConfiguracionGeneral.Commands.ActualizarCertificadoSat;
using Core.Application.ConfiguracionGeneral.Models;
using Core.Application.ConfiguracionGeneral.Queries.BuscarConfiguracionGeneral;
using MahApps.Metro.Controls.Dialogs;
using MediatR;
using Microsoft.Win32;

namespace Presentation.WpfApp.ViewModels.ConfiguracionGeneral
{
    public sealed class ConfiguracionGeneralViewModel : Screen
    {
        private readonly IDialogCoordinator _dialogCoordinator;
        private readonly IMediator _mediator;

        public ConfiguracionGeneralViewModel(IMediator mediator, IDialogCoordinator dialogCoordinator)
        {
            _mediator = mediator;
            _dialogCoordinator = dialogCoordinator;
            DisplayName = "Configuracion General";
        }

        public ConfiguracionGeneralDto ConfiguracionGeneral { get; set; }

        public async Task InicializarAsync()
        {
            await CargarConfiguracionGeneralAsync();
        }

        private async Task CargarConfiguracionGeneralAsync()
        {
            ConfiguracionGeneral = await _mediator.Send(new BuscarConfiguracionGeneralQuery());
        }

        public async Task BuscarArchivoCertificadoAsync()
        {
            try
            {
                var openFileDialog = new OpenFileDialog();
                var dialogResult = openFileDialog.ShowDialog();
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
                await _mediator.Send(new ActualizarCertificadoSatCommand(
                    ConfiguracionGeneral.CertificadoSat.Certificado,
                    ConfiguracionGeneral.CertificadoSat.Contrasena,
                    ConfiguracionGeneral.CertificadoSat.RfcEmisor,
                    ConfiguracionGeneral.RutaDirectorioDescargas));

                TryClose();
            }
            catch (Exception e)
            {
                await _dialogCoordinator.ShowMessageAsync(this, "Error", e.ToString());
            }
        }

        public void Cancelar()
        {
            TryClose();
        }
    }
}