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
    string? Descripcion = null,
    /// <summary>
    /// Campana a la que se imputa (historia 2-2). Opcional: una donacion
    /// espontanea no responde a ninguna convocatoria y sigue siendo valida.
    /// </summary>
    Guid? CampanaDonacionId = null) : IRequest<Guid>;
