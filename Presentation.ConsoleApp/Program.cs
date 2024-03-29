﻿using System.CommandLine;
using System.CommandLine.Invocation;
using Common.DateRanges;
using Core.Application.Autenticacion.Queries.ValidarCredencialesUsuario;
using Core.Application.Common;
using Core.Application.ConfiguracionGeneral.Models;
using Core.Application.ConfiguracionGeneral.Queries.BuscarConfiguracionGeneral;
using Core.Application.Empresas.Models;
using Core.Application.Empresas.Queries.BuscarEmpresaPorNombre;
using Core.Application.Solicitudes.Commands.CrearSolicitud;
using Core.Application.Solicitudes.Commands.ProcesarSolicitud;
using Core.Application.Usuarios.Models;
using Infrastructure.Common;
using Infrastructure.Persistance.Common;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog;
using Presentation.ConsoleApp.Models;

namespace Presentation.ConsoleApp;

public class Program
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    private static IHost _host;

    public static async Task Main(string[] args)
    {
        Logger.Info(Environment.CommandLine);

        _host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, serviceCollection) =>
            {
                serviceCollection.AddApplicationServices();
                serviceCollection.AddInfrastructureServices();
                serviceCollection.AddPersistenceServices(context.Configuration);
            })
            .Build();

        await _host.StartAsync();

        try
        {
            Logger.Info("Inicializando aplicacion.");
            var crearCommand = new Command("crear")
            {
                new Option<TipoRangoFechaEnum>("--tipo-rango-fecha", () => TipoRangoFechaEnum.Hoy),
                new Option<DateTime>("--fecha-inicio", () => DateTime.Today),
                new Option<DateTime>("--fecha-fin", () => DateTime.Today),
                new Option<TipoSolicitudEnum>("--tipo-solicitud", () => TipoSolicitudEnum.Recibidos),
                new Option<bool>("--procesar", () => false)
            };
            crearCommand.Handler = CommandHandler.Create<CrearCommandOptions>(CrearCommandActionAsync);

            var procesarCommand = new Command("procesar") { new Option<int>("--solicitud-id") };
            procesarCommand.Handler = CommandHandler.Create<ProcesarCommandOptions>(ProcesarCommandActionAsync);

            var command = new RootCommand
            {
                new Option<string>(new[] { "--usuario-nombre", "-u" }),
                new Option<string>(new[] { "--usuario-contrasena", "-c" }),
                new Option<string>(new[] { "--empresa-nombre", "-e" }),
                crearCommand,
                procesarCommand
            };

            //await command.InvokeAsync(@"-u admin -c admin -e ""EMPRESA PRUEBA"" crear --tipo-rango-fecha Hoy");
            await command.InvokeAsync(args);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        Console.ReadLine();
    }

    private static void ProcesarCommandActionAsync(ProcesarCommandOptions obj)
    {
        Console.WriteLine("Dentro de prcesar command actions");
    }

    private static async Task CrearCommandActionAsync(CrearCommandOptions crearCommandOptions)
    {
        var mediator = _host.Services.GetRequiredService<IMediator>();

        Logger.Info("Validando credenciales de usuario.");
        UsuarioDto usuario =
            await mediator.Send(new ValidarCredencialesUsuarioQuery(crearCommandOptions.UsuarioNombre,
                crearCommandOptions.UsuarioContrasena));
        if (usuario == null)
            throw new ArgumentException("La credencales proporcionadas no son validas.", nameof(crearCommandOptions.UsuarioNombre));

        Logger.Info("Buscando empresa.");
        EmpresaPerfilDto empresa = await mediator.Send(new BuscarEmpresaPorNombreQuery(crearCommandOptions.EmpresaNombre));
        if (empresa == null)
            throw new ArgumentException("No se encontro la empresa.", nameof(crearCommandOptions.EmpresaNombre));

        Logger.Info("Buscando configuracion general.");
        ConfiguracionGeneralDto configuracionEmpresa = await mediator.Send(new BuscarConfiguracionGeneralQuery(empresa.Id));

        DateTime fechaInicio;
        DateTime fechaFin;
        switch (crearCommandOptions.TipoRangoFecha)
        {
            case TipoRangoFechaEnum.Custumizado:
                fechaInicio = crearCommandOptions.FechaInicio;
                fechaFin = crearCommandOptions.FechaFin;
                break;
            case TipoRangoFechaEnum.Hoy:
                fechaInicio = RangoFecha.Hoy.Inicio;
                fechaFin = RangoFecha.Hoy.Fin;
                break;
            case TipoRangoFechaEnum.Ayer:
                fechaInicio = RangoFecha.Ayer.Inicio;
                fechaFin = RangoFecha.Ayer.Fin;
                break;
            case TipoRangoFechaEnum.EstaSemana:
                fechaInicio = RangoFecha.EstaSemana.Inicio;
                fechaFin = RangoFecha.EstaSemana.Fin;
                break;
            case TipoRangoFechaEnum.EstaSemanaAlDia:
                fechaInicio = RangoFecha.EstaSemanaAlDia.Inicio;
                fechaFin = RangoFecha.EstaSemanaAlDia.Fin;
                break;
            case TipoRangoFechaEnum.EsteMes:
                fechaInicio = RangoFecha.EsteMes.Inicio;
                fechaFin = RangoFecha.EsteMes.Fin;
                break;
            case TipoRangoFechaEnum.EsteMesAlDia:
                fechaInicio = RangoFecha.EsteMesAlDia.Inicio;
                fechaFin = RangoFecha.EsteMesAlDia.Fin;
                break;
            case TipoRangoFechaEnum.EsteAno:
                fechaInicio = RangoFecha.EsteAno.Inicio;
                fechaFin = RangoFecha.EsteAno.Fin;
                break;
            case TipoRangoFechaEnum.EsteAnoAlDia:
                fechaInicio = RangoFecha.EsteAnoAlDia.Inicio;
                fechaFin = RangoFecha.EsteAnoAlDia.Fin;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        string rfcEmisor;
        string rfcReceptor;
        switch (crearCommandOptions.TipoSolicitud)
        {
            case TipoSolicitudEnum.Recibidos:
                rfcEmisor = "";
                rfcReceptor = configuracionEmpresa.CertificadoSat.Rfc;
                break;
            case TipoSolicitudEnum.Emitidos:
                rfcEmisor = configuracionEmpresa.CertificadoSat.Rfc;
                rfcReceptor = "";
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        Logger.Info("Creando solicitud.");
        int solicitudId = await mediator.Send(new CrearSolicitudCommand(empresa.Id,
            usuario.Id,
            fechaInicio,
            fechaFin,
            rfcEmisor,
            rfcReceptor,
            configuracionEmpresa.CertificadoSat.Rfc,
            "CFDI",
            crearCommandOptions.Uuid));

        if (solicitudId != 0 && crearCommandOptions.Procesar)
        {
            Logger.Info("Procesando solicitud con id {0}.", solicitudId);
            await mediator.Send(new ProcesarSolicitudCommand(solicitudId, usuario.Id));
        }
    }
}
