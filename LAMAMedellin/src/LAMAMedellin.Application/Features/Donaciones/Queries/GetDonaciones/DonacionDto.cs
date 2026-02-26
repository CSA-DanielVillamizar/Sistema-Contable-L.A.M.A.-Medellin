using LAMAMedellin.Domain.Enums;

namespace LAMAMedellin.Application.Features.Donaciones.Queries.GetDonaciones;

public sealed record DonacionDto(
    Guid Id,
    Guid DonanteId,
    string NombreDonante,
    decimal MontoCOP,
    DateTime Fecha,
    Guid BancoId,
    Guid CentroCostoId,
    bool CertificadoEmitido,
    string CodigoVerificacion,
    FormaDonacion FormaDonacion,
    string MedioPagoODescripcion);
