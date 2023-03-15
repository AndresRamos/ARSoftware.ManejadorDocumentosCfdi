using Core.Application.ConfiguracionGeneral.Models;
using MediatR;

namespace Core.Application.ConfiguracionGeneral.Commands.ActualizarConfiguracionGeneral;

public sealed record ActualizarConfiguracionGeneralCommand(int EmpresaId, ConfiguracionGeneralDto ConfiguracionGeneral) : IRequest;
