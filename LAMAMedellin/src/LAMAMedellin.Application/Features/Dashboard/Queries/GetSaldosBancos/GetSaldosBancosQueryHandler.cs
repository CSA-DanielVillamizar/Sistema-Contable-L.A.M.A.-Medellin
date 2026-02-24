using LAMAMedellin.Application.Common.Interfaces.Repositories;
using MediatR;

namespace LAMAMedellin.Application.Features.Dashboard.Queries.GetSaldosBancos;

public sealed class GetSaldosBancosQueryHandler(
    IBancoRepository bancoRepository)
    : IRequestHandler<GetSaldosBancosQuery, List<SaldoBancoDto>>
{
    public async Task<List<SaldoBancoDto>> Handle(
        GetSaldosBancosQuery request,
        CancellationToken cancellationToken)
    {
        var bancos = await bancoRepository.GetAllAsync(cancellationToken);

        return bancos
            .Select(banco => new SaldoBancoDto(
                banco.NumeroCuenta,
                banco.SaldoActual))
            .ToList();
    }
}
