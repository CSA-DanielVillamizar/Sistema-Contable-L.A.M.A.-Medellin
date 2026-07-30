using MediatR;

namespace LAMAMedellin.Application.Features.Donaciones.Queries.GetDonaciones;

/// <summary>
/// Donaciones con filtros (historia 2-4). La consulta no admitia ninguno, de
/// modo que la pantalla traia todo el historico y filtrar quedaba a cargo del
/// navegador: sirve con cien donaciones y deja de servir con diez mil.
///
/// Todos los filtros son opcionales y se combinan entre si.
/// </summary>
public sealed record GetDonacionesQuery(
    DateOnly? Desde = null,
    DateOnly? Hasta = null,
    Guid? DonanteId = null,
    Guid? CentroCostoId = null,
    bool? CertificadoEmitido = null) : IRequest<IReadOnlyList<DonacionDto>>;
