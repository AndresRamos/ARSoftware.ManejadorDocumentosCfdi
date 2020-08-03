using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Autofac;
using Common.DateRanges;
using Core.Application.Autenticacion.Queries.ValidarCredencialesUsuario;
using Core.Application.Empresas.Queries.BuscarEmpresaPorId;
using Core.Application.Solicitudes.Commands.CrearSolicitud;
using Core.Application.Solicitudes.Commands.ProcesarSolicitud;
using MediatR;
using Presentation.ConsoleApp.Config;
using Presentation.ConsoleApp.Models;

namespace Presentation.ConsoleApp
{
    public class Program
    {
        private static IContainer _container;

        public static async Task Main(string[] args)
        {
            try
            {
                var crearCommand = new Command("crear")
                {
                    new Option<TipoRangoFechaEnum>("--tipo-rango-fecha", () => TipoRangoFechaEnum.Hoy),
                    new Option<DateTime>("--fecha-inicio", () => DateTime.Today),
                    new Option<DateTime>("--fecha-fin", () => DateTime.Today),
                    new Option<TipoSolicitudEnum>("--tipo-solicitud", () => TipoSolicitudEnum.Recibidos)
                };
                crearCommand.Handler = CommandHandler.Create<CrearCommandOptions>(CrearCommandActionAsync);

                var procesarCommand = new Command("procesar")
                {
                    new Option<int>("--solicitud-id")
                };
                procesarCommand.Handler = CommandHandler.Create<ProcesarCommandOptions>(ProcesarCommandActionAsync);

                var command = new RootCommand
                {
                    new Option<string>(new[] {"--usuario-nombre", "-u"}),
                    new Option<string>(new[] {"--usuario-contrasena", "-c"}),
                    new Option<string>(new[] {"--empresa-nombre", "-e"}),
                    crearCommand,
                    procesarCommand
                };

                _container = IocContainerConfig.Configure();

                var result = await command.InvokeAsync("-u usuario -c contrasena -e empresa crear");
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

        private static async Task CrearCommandActionAsync(CrearCommandOptions options)
        {
            var mediator = _container.Resolve<IMediator>();

            var usuario = await mediator.Send(new ValidarCredencialesUsuarioQuery(options.UsuarioNombre, options.UsuarioContrasena));
            if (usuario == null)
            {
                throw new ArgumentException("La credencales proporcionadas no son validas.", nameof(options.UsuarioNombre));
            }

            var empresa = await mediator.Send(new BuscarEmpresaPorIdQuery(1));
            if (empresa == null)
            {
                throw new ArgumentException("No se encontro la empresa.", nameof(options.EmpresaNombre));
            }

            var solicitudId = await mediator.Send(new CrearSolicitudCommand(empresa.Id, usuario.Id, options.FechaInicio, options.FechaFin, "", "", "", "CFDI"));

            if (true && solicitudId != 0)
            {
                await mediator.Send(new ProcesarSolicitudCommand(solicitudId));
            }
        }
    }
}