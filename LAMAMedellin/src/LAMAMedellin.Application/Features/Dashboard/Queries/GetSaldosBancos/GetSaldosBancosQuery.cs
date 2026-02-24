using MediatR;

namespace LAMAMedellin.Application.Features.Dashboard.Queries.GetSaldosBancos;

public sealed record GetSaldosBancosQuery : IRequest<List<SaldoBancoDto>>;
