using System.Data.Entity;
using System.Data.Entity.Core;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Core.Application.Solicitudes.Commands.AutenticarSolicitud;
using Core.Application.Solicitudes.Commands.DescargarSolicitud;
using Core.Application.Solicitudes.Commands.GenerarSolicitud;
using Core.Application.Solicitudes.Commands.VerificarSolicitud;
using Core.Domain.Entities;
using Infrastructure.Persistance;
using MediatR;
using NLog;

namespace Core.Application.Solicitudes.Commands.ProcesarSolicitud
{
    public class ProcesarSolicitudCommandHandler : IRequestHandler<ProcesarSolicitudCommand>
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ManejadorDocumentosCfdiDbContext _context;
        private readonly IMediator _mediator;

        public ProcesarSolicitudCommandHandler(IMediator mediator, ManejadorDocumentosCfdiDbContext context)
        {
            _mediator = mediator;
            _context = context;
        }

        public async Task<Unit> Handle(ProcesarSolicitudCommand request, CancellationToken cancellationToken)
        {
            Logger.WithProperty(LogPropertyConstants.SolicitudId, request.SolicitudId).Info("Procesando solicitud {0}", request.SolicitudId);

            var solicitud = await BuscarSolicitudAsync(request.SolicitudId, cancellationToken);

            // Autenticar
            if (solicitud.SolicitudAutenticacion == null || !solicitud.SolicitudAutenticacion.IsTokenValido)
            {
                Logger.WithProperty(LogPropertyConstants.SolicitudId, solicitud.Id).Info("Autenticando solicitud {0}", solicitud.Id);
                await _mediator.Send(new AutenticarSolicitudCommand(solicitud.Id), cancellationToken);
                solicitud = await BuscarSolicitudAsync(solicitud.Id, cancellationToken);
            }

            // Generar Solicitud
            if (solicitud.SolicitudAutenticacion.IsTokenValido && (solicitud.SolicitudSolicitud == null || !solicitud.SolicitudSolicitud.IsValid))
            {
                Logger.WithProperty(LogPropertyConstants.SolicitudId, solicitud.Id).Info("Generando solicitud {0}", solicitud.Id);
                await _mediator.Send(new GenerarSolicitudCommand(solicitud.Id), cancellationToken);
                solicitud = await BuscarSolicitudAsync(solicitud.Id, cancellationToken);
            }

            // Verificar Solicitud
            int tries = 0;
            if (solicitud.SolicitudAutenticacion.IsTokenValido && solicitud.SolicitudSolicitud.IsValid && (solicitud.SolicitudVerificacion == null || !solicitud.SolicitudVerificacion.IsValid))
            {
                do
                {
                    await Task.Delay(30000, cancellationToken);
                    Logger.WithProperty(LogPropertyConstants.SolicitudId, solicitud.Id).Info("Verificando solicitud {0}", solicitud.Id);
                    await _mediator.Send(new VerificarSolicitudCommand(solicitud.Id), cancellationToken);
                    solicitud = await BuscarSolicitudAsync(solicitud.Id, cancellationToken);
                    tries++;
                } while ((solicitud.SolicitudVerificacion.EstadoSolicitud == "1" || solicitud.SolicitudVerificacion.EstadoSolicitud == "2") && tries < 3);
            }

            // Descargar Solicitud
            solicitud = await BuscarSolicitudAsync(solicitud.Id, cancellationToken);
            if (solicitud.SolicitudAutenticacion.IsTokenValido && solicitud.SolicitudSolicitud.IsValid && solicitud.SolicitudVerificacion.IsValid && solicitud.SolicitudVerificacion.HasPaquetesPendientesPorDescargar)
            {
                Logger.WithProperty(LogPropertyConstants.SolicitudId, solicitud.Id).Info("Descargando solicitud {0}", solicitud.Id);
                await _mediator.Send(new DescargarSolicitudCommand(solicitud.Id), cancellationToken);
                solicitud = await BuscarSolicitudAsync(solicitud.Id, cancellationToken);
            }

            Logger.WithProperty(LogPropertyConstants.SolicitudId, solicitud.Id).Info("La solicitud {0} fue procesada.", solicitud.Id);

            return Unit.Value;
        }

        private async Task<Solicitud> BuscarSolicitudAsync(int solicitudId, CancellationToken cancellationToken)
        {
            var solicitud = await _context.Solicitudes
                .Include(s => s.SolicitudAutenticacion)
                .Include(s => s.SolicitudSolicitud)
                .Include(s => s.SolicitudVerificacion.PaquetesIds)
                .AsNoTracking()
                .SingleOrDefaultAsync(s => s.Id == solicitudId, cancellationToken);

            if (solicitud == null)
            {
                Logger.Error("No se encontrol la solicitud con Id {0}", solicitudId);
                throw new ObjectNotFoundException($"No se encontrol la solicitud con Id {solicitudId}.");
            }

            return solicitud;
        }
    }
}