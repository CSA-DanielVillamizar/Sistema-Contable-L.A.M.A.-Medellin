using LAMAMedellin.Application.Features.Migracion.Commands.ImportarHistorico;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LAMAMedellin.WebApi.Controllers;

/// <summary>
/// Controlador para operaciones de migración de datos históricos.
/// Solo accesible para usuarios con rol Admin.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class MigracionController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ILogger<MigracionController> _logger;

    public MigracionController(ISender sender, ILogger<MigracionController> logger)
    {
        _sender = sender;
        _logger = logger;
    }

    /// <summary>
    /// Importa datos históricos desde archivo CSV (docs/Historico.csv).
    /// Genera comprobantes contables automáticamente aplicando reglas de partida doble.
    /// </summary>
    /// <remarks>
    /// **ADVERTENCIA**: Esta operación crea múltiples comprobantes en la base de datos.
    /// Solo ejecutar UNA VEZ sobre base limpia para evitar duplicados.
    ///
    /// Reglas de negocio aplicadas:
    /// - SALDO EFECTIVO: Débito Caja (110505), Crédito Patrimonio (370505)
    /// - INGRESO: Débito Caja (110505), Crédito Ingresos según concepto
    /// - EGRESO: Débito Gastos según concepto, Crédito Caja (110505)
    ///
    /// Asociación de terceros:
    /// - Busca miembros activos por nombre/apellido en el concepto
    /// - Usa tercero genérico GUID 00000000-0000-0000-0000-000000000001 cuando no hay match
    /// </remarks>
    /// <response code="200">Importación exitosa con resumen de registros procesados</response>
    /// <response code="401">No autenticado</response>
    /// <response code="403">No autorizado (requiere rol Admin)</response>
    /// <response code="500">Error al procesar archivo CSV o crear comprobantes</response>
    [HttpPost("cargar-historico")]
    [ProducesResponseType(typeof(ImportarHistoricoResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ImportarHistoricoResult>> CargarHistorico(
        [FromBody] ImportarHistoricoCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogWarning(
            "Iniciando importación histórica por usuario {UserId} con rol {Role}",
            User.FindFirst("oid")?.Value ?? "Unknown",
            User.FindFirst("roles")?.Value ?? "Unknown");

        var resultado = await _sender.Send(command, cancellationToken);

        if (resultado.Advertencias.Any())
        {
            _logger.LogWarning(
                "Importación completada con {Count} advertencias: {Advertencias}",
                resultado.Advertencias.Count,
                string.Join("; ", resultado.Advertencias.Take(5)));
        }

        return Ok(resultado);
    }
}
