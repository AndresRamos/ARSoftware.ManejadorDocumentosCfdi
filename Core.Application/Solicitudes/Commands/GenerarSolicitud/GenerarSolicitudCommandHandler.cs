﻿using System.Data.Entity;
using System.Security.Cryptography.X509Certificates;
using ARSoftware.Cfdi.DescargaMasiva.Enumerations;
using ARSoftware.Cfdi.DescargaMasiva.Helpers;
using ARSoftware.Cfdi.DescargaMasiva.Interfaces;
using ARSoftware.Cfdi.DescargaMasiva.Models;
using Common;
using Core.Domain.Entities;
using Infrastructure.Persistance;
using MediatR;
using NLog;

namespace Core.Application.Solicitudes.Commands.GenerarSolicitud;

public sealed class GenerarSolicitudCommandHandler : IRequestHandler<GenerarSolicitudCommand>
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    private readonly ManejadorDocumentosCfdiDbContext _context;
    private readonly ISolicitudService _solicitudService;

    public GenerarSolicitudCommandHandler(ManejadorDocumentosCfdiDbContext context, ISolicitudService solicitudService)
    {
        _context = context;
        _solicitudService = solicitudService;
    }

    public async Task Handle(GenerarSolicitudCommand request, CancellationToken cancellationToken)
    {
        Logger.WithProperty(LogPropertyConstants.SolicitudId, request.SolicitudId).Info("Generando solicitud {0}", request.SolicitudId);

        Domain.Entities.ConfiguracionGeneral configuracionGeneral = await _context.ConfiguracionGeneral.FirstAsync(cancellationToken);
        Solicitud solicitud = await _context.Solicitudes.Include(s => s.SolicitudAutenticacion)
            .Include(s => s.SolicitudSolicitud)
            .Include(s => s.SolicitudesWeb)
            .SingleAsync(s => s.Id == request.SolicitudId, cancellationToken);

        if (solicitud.SolicitudAutenticacion == null)
        {
            Logger.WithProperty(LogPropertyConstants.SolicitudId, request.SolicitudId)
                .Error("No se puede generar la solicitud {0} por que no existe una solicitud de autenticacion.", solicitud.Id);
            throw new InvalidOperationException(
                $"No se puede generar la solicitud {solicitud.Id} por que no existe una solicitud de autenticacion.");
        }

        if (!solicitud.SolicitudAutenticacion.IsTokenValido)
        {
            Logger.WithProperty(LogPropertyConstants.SolicitudId, request.SolicitudId)
                .Error("No se puede generar la solicitud {0} por que el token no es valido.", solicitud.Id);
            throw new InvalidOperationException($"No se puede generar la solicitud {solicitud.Id} por que el token no es valido.");
        }

        Logger.WithProperty(LogPropertyConstants.SolicitudId, request.SolicitudId).Info("Obteniendo certificado.");
        X509Certificate2 certificadoSat = X509Certificate2Helper.GetCertificate(configuracionGeneral.CertificadoSat.Certificado,
            configuracionGeneral.CertificadoSat.Contrasena);

        Logger.WithProperty(LogPropertyConstants.SolicitudId, solicitud.Id).Info("Generando XML SOAP de solicitud.");

        SolicitudRequest solicitudRequest;

        if (!string.IsNullOrWhiteSpace(solicitud.Uuid))
            // Buscar solicitud por UUID
            solicitudRequest = SolicitudRequest.CreateInstance(solicitud.Uuid,
                solicitud.RfcSolicitante,
                AccessToken.CreateInstance(solicitud.SolicitudAutenticacion.Token));
        else
            solicitudRequest = SolicitudRequest.CreateInstance(solicitud.FechaInicio,
                solicitud.FechaFin,
                solicitud.TipoSolicitud == TipoSolicitud.Cfdi.Name ? TipoSolicitud.Cfdi :
                solicitud.TipoSolicitud == TipoSolicitud.Metadata.Name ? TipoSolicitud.Metadata :
                throw new ArgumentException("El tipo de solicitud no es un tipo valido."),
                solicitud.RfcEmisor ?? "",
                solicitud.Receptores,
                solicitud.RfcSolicitante,
                AccessToken.CreateInstance(solicitud.SolicitudAutenticacion.Token));

        string soapRequestEnvelopeXml = _solicitudService.GenerateSoapRequestEnvelopeXmlContent(solicitudRequest, certificadoSat);
        Logger.WithProperty(LogPropertyConstants.SolicitudId, solicitud.Id).Info("SoapRequestEnvelopeXml: {0}", soapRequestEnvelopeXml);

        SolicitudResult solicitudResult;
        try
        {
            Logger.WithProperty(LogPropertyConstants.SolicitudId, solicitud.Id).Info("Enviando solicitud SOAP de generacion.");
            SoapRequestResult soapRequestResult = await _solicitudService.SendSoapRequestAsync(soapRequestEnvelopeXml,
                AccessToken.CreateInstance(solicitud.SolicitudAutenticacion.Token),
                cancellationToken);

            solicitudResult = _solicitudService.GetSoapResponseResult(soapRequestResult);
        }
        catch (Exception e)
        {
            Logger.WithProperty(LogPropertyConstants.SolicitudId, solicitud.Id)
                .Error(e, "Se produjo un error al enviar la solicitud SOAP de generacion.");

            var solicitudSolicitudError = SolicitudSolicitud.CreateInstance(soapRequestEnvelopeXml,
                null,
                solicitud.FechaInicio,
                solicitud.FechaFin,
                solicitud.RfcEmisor,
                solicitud.RfcReceptor,
                solicitud.RfcSolicitante,
                solicitud.TipoSolicitud,
                null,
                null,
                null,
                e.ToString());

            solicitud.SolicitudesWeb.Add(solicitudSolicitudError);
            solicitud.SolicitudSolicitud = solicitudSolicitudError;
            await _context.SaveChangesAsync(cancellationToken);

            throw;
        }

        Logger.WithProperty(LogPropertyConstants.SolicitudId, solicitud.Id)
            .Info("GenerarSolicitudResult: {@GenerarSolicitudResult}", solicitudResult);

        Logger.WithProperty(LogPropertyConstants.SolicitudId, solicitud.Id).Info("Creando registro de solicitud de solicitud.");
        var solicitudSolicitud = SolicitudSolicitud.CreateInstance(soapRequestEnvelopeXml,
            solicitudResult.ResponseContent,
            solicitud.FechaInicio,
            solicitud.FechaFin,
            solicitud.RfcEmisor,
            solicitud.RfcReceptor,
            solicitud.RfcSolicitante,
            solicitud.TipoSolicitud,
            solicitudResult.RequestStatusCode,
            solicitudResult.RequestStatusMessage,
            solicitudResult.RequestId,
            null);

        Logger.WithProperty(LogPropertyConstants.SolicitudId, solicitud.Id)
            .Info("SolicitudSolicitud: {@SolicitudSolicitud}", solicitudSolicitud);

        solicitud.SolicitudesWeb.Add(solicitudSolicitud);
        solicitud.SolicitudSolicitud = solicitudSolicitud;

        Logger.WithProperty(LogPropertyConstants.SolicitudId, solicitud.Id).Info("Guardando cambios.");
        await _context.SaveChangesAsync(cancellationToken);
    }
}
