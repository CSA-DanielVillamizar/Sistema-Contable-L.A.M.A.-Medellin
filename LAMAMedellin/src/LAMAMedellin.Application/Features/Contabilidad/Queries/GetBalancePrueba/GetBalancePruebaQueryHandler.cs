using LAMAMedellin.Application.Common.Interfaces.Repositories;
using MediatR;

namespace LAMAMedellin.Application.Features.Contabilidad.Queries.GetBalancePrueba;

public sealed class GetBalancePruebaQueryHandler(ILibrosContablesRepository librosRepository)
    : IRequestHandler<GetBalancePruebaQuery, BalancePruebaDto>
{
    public async Task<BalancePruebaDto> Handle(GetBalancePruebaQuery request, CancellationToken cancellationToken)
    {
        var cuentas = await librosRepository.GetBalancePruebaAsync(
            request.Anio,
            request.Mes,
            request.CentroCostoId,
            cancellationToken);

        var totalDebe = cuentas.Sum(c => c.Debe);
        var totalHaber = cuentas.Sum(c => c.Haber);

        return new BalancePruebaDto(
            request.Anio,
            request.Mes,
            totalDebe,
            totalHaber,
            totalDebe == totalHaber,
            cuentas);
    }
}
