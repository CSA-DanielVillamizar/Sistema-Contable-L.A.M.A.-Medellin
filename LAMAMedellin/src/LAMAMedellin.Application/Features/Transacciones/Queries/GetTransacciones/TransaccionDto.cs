namespace LAMAMedellin.Application.Features.Transacciones.Queries.GetTransacciones;

public sealed record TransaccionDto(
    Guid Id,
    DateTime Fecha,
    string Tipo,
    decimal MontoCOP,
    string Descripcion,
    string CentroCosto,
    string Banco);
