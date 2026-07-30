using MediatR;

namespace LAMAMedellin.Application.Features.Donaciones.Queries.GetCampanas;

public sealed record GetCampanasQuery(bool IncluirCerradas = true)
    : IRequest<IReadOnlyList<CampanaDonacionDto>>;

/// <summary>
/// Una campana con su avance. Lo recaudado se calcula sumando las donaciones
/// imputadas, no se guarda: un total almacenado se desincroniza en cuanto
/// alguien corrige una donacion, y nadie se entera.
/// </summary>
public sealed record CampanaDonacionDto(
    Guid Id,
    string Nombre,
    string Descripcion,
    decimal MetaCOP,
    decimal RecaudadoCOP,
    decimal PorcentajeAvance,
    int CantidadDonaciones,
    DateOnly FechaInicio,
    DateOnly FechaFin,
    bool EstaActiva,
    bool EstaVigente);
