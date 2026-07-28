using MediatR;

namespace LAMAMedellin.Application.Features.Contabilidad.Queries.GetLibroMayor;

public sealed record GetLibroMayorQuery(
    Guid CuentaContableId,
    DateOnly Desde,
    DateOnly Hasta,
    Guid? CentroCostoId = null) : IRequest<LibroMayorDto>;
