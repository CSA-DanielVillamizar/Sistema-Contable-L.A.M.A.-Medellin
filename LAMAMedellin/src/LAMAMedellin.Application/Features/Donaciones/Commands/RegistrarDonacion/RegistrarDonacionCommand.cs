using LAMAMedellin.Domain.Enums;
using MediatR;

namespace LAMAMedellin.Application.Features.Donaciones.Commands.RegistrarDonacion;

public sealed record RegistrarDonacionCommand(
    Guid DonanteId,
    decimal MontoCOP,
    Guid BancoId,
    Guid CentroCostoId,
    MedioPago MedioPago,
    FormaDonacion FormaDonacion,
    string MedioPagoODescripcion,
    string? Descripcion = null) : IRequest<Guid>;
