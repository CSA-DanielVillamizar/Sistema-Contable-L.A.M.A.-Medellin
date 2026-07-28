using MediatR;

namespace LAMAMedellin.Application.Features.Contabilidad.Queries.GetBalancePrueba;

public sealed record GetBalancePruebaQuery(
    int Anio,
    int Mes,
    Guid? CentroCostoId = null) : IRequest<BalancePruebaDto>;
