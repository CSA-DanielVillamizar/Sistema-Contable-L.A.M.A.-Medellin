using LAMAMedellin.Application.Common.Interfaces.Repositories;
using MediatR;

namespace LAMAMedellin.Application.Features.Tesoreria.Queries.GetCuentasBancarias;

public sealed class GetCuentasBancariasQueryHandler(IBancoRepository bancoRepository)
    : IRequestHandler<GetCuentasBancariasQuery, IReadOnlyList<CuentaBancariaDto>>
{
    public async Task<IReadOnlyList<CuentaBancariaDto>> Handle(
        GetCuentasBancariasQuery request,
        CancellationToken cancellationToken)
    {
        var bancos = await bancoRepository.GetAllAsync(cancellationToken);

        return bancos
            .Where(b => request.IncluirInactivas || b.EsActivo)
            .OrderBy(b => b.Nombre)
            .Select(b => new CuentaBancariaDto(b.Id, b.Nombre, b.NumeroCuenta, b.SaldoActual, b.EsActivo, b.CuentaContableId))
            .ToList();
    }
}
