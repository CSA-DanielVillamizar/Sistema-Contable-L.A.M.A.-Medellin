using LAMAMedellin.Domain.Enums;

namespace LAMAMedellin.Application.Features.Donaciones.Queries.GetDonaciones;

/// <summary>
/// Donacion tal como la lista la pantalla y la exportacion CSV.
///
/// Lleva el nombre del banco y del centro de costo ademas de sus
/// identificadores: el frontend ya los pedia —los mapeaba como `banco` y
/// `centroCosto`— pero el contrato solo traia los Guid, asi que llegaban
/// siempre vacios. Un identificador no le dice nada a quien lee el reporte.
/// </summary>
public sealed record DonacionDto(
    Guid Id,
    Guid DonanteId,
    string NombreDonante,
    decimal MontoCOP,
    DateTime Fecha,
    Guid BancoId,
    string Banco,
    Guid CentroCostoId,
    string CentroCosto,
    bool CertificadoEmitido,
    string CodigoVerificacion,
    FormaDonacion FormaDonacion,
    string MedioPagoODescripcion);
