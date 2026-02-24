namespace LAMAMedellin.Application.Features.Cartera.Queries.GetCarteraPendiente;

public sealed record CarteraPendienteDto(
    Guid Id,
    Guid MiembroId,
    string NombreMiembro,
    string Periodo,
    decimal ValorEsperadoCOP,
    decimal SaldoPendienteCOP);
