using LAMAMedellin.Domain.Enums;
using MediatR;

namespace LAMAMedellin.Application.Features.Transacciones.Commands.RegistrarIngreso;

public sealed record RegistrarIngresoCommand(
    decimal MontoCOP,
    Guid CentroCostoId,
    Guid BancoId,
    MedioPago MedioPago,
    string Descripcion,
    string? MonedaOrigen = null,
    decimal? MontoMonedaOrigen = null,
    decimal? TasaCambioUsada = null,
    DateTime? FechaTasaCambio = null,
    FuenteTasaCambio? FuenteTasaCambio = null) : IRequest<Guid>;
