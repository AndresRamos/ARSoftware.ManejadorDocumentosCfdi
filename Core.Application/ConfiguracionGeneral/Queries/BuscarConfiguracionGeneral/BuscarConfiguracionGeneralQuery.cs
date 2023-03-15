using Core.Application.ConfiguracionGeneral.Models;
using MediatR;

namespace Core.Application.ConfiguracionGeneral.Queries.BuscarConfiguracionGeneral;

public sealed record BuscarConfiguracionGeneralQuery(int EmpresaId) : IRequest<ConfiguracionGeneralDto>;
