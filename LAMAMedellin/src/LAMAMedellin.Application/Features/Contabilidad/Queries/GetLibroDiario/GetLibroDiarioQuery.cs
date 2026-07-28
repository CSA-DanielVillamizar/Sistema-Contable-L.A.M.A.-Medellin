using MediatR;

namespace LAMAMedellin.Application.Features.Contabilidad.Queries.GetLibroDiario;

public sealed record GetLibroDiarioQuery(
    DateOnly Desde,
    DateOnly Hasta,
    Guid? CentroCostoId = null) : IRequest<LibroDiarioDto>;
