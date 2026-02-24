using MediatR;

namespace LAMAMedellin.Application.Features.Cartera.Commands.RegistrarPagoCartera;

public sealed record RegistrarPagoCarteraCommand(
    Guid CarteraId,
    decimal MontoPagadoCOP,
    Guid BancoId,
    Guid CentroCostoId,
    string? Descripcion = null) : IRequest<Unit>;
