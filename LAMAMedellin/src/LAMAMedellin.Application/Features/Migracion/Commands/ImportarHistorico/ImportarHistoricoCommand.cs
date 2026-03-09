using MediatR;

namespace LAMAMedellin.Application.Features.Migracion.Commands.ImportarHistorico;

/// <summary>
/// Comando para importar datos históricos desde CSV (Enero 2025 - Febrero 2026).
/// Recibe el contenido del CSV y genera comprobantes contables automáticamente.
/// </summary>
public sealed record ImportarHistoricoCommand : IRequest<ImportarHistoricoResult>
{
    public string ContenidoCsv { get; init; } = string.Empty;
}

public sealed record ImportarHistoricoResult(
    int ComprobantesCreados,
    int LineasProcesadas,
    List<string> Advertencias);
