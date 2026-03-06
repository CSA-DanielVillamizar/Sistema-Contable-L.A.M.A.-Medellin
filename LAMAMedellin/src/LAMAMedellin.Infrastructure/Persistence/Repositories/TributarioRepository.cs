using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Application.Features.Tributario.Queries.GetReporteCalidadDatos;
using LAMAMedellin.Application.Features.Tributario.Queries.GetReporteBeneficiariosFinales;
using LAMAMedellin.Application.Features.Tributario.Queries.GetReporteExogena;
using LAMAMedellin.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace LAMAMedellin.Infrastructure.Persistence.Repositories;

public sealed class TributarioRepository(LamaDbContext context) : ITributarioRepository
{
    private readonly LamaDbContext _context = context;

    public async Task<IReadOnlyList<InconsistenciaTributariaDto>> GetReporteCalidadDatosAsync(CancellationToken cancellationToken = default)
    {
        var inconsistenciasMiembros = await _context.Miembros
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .Where(x => string.IsNullOrWhiteSpace(x.Documento))
            .Select(x => new InconsistenciaTributariaDto(
                x.Id.ToString(),
                $"{x.Nombre} {x.Apellidos}".Trim(),
                "Miembro",
                "Falta Número de Documento"))
            .ToListAsync(cancellationToken);

        var inconsistenciasDonantesNumeroDocumento = await _context.Donantes
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .Where(x => string.IsNullOrWhiteSpace(x.NumeroDocumento))
            .Select(x => new InconsistenciaTributariaDto(
                x.Id.ToString(),
                x.NombreORazonSocial,
                "Donante",
                "Falta Número de Documento"))
            .ToListAsync(cancellationToken);

        var inconsistenciasDonantesTipoDocumento = await _context.Donantes
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .Where(x => x.TipoDocumento == TipoDocumentoDonante.Otro)
            .Select(x => new InconsistenciaTributariaDto(
                x.Id.ToString(),
                x.NombreORazonSocial,
                "Donante",
                "Falta Tipo de Documento"))
            .ToListAsync(cancellationToken);

        return inconsistenciasMiembros
            .Concat(inconsistenciasDonantesNumeroDocumento)
            .Concat(inconsistenciasDonantesTipoDocumento)
            .OrderBy(x => x.TipoRelacion)
            .ThenBy(x => x.NombreObtenido)
            .ThenBy(x => x.DescripcionInconsistencia)
            .ToList();
    }

    public async Task<IReadOnlyList<BeneficiarioFinalDto>> GetReporteBeneficiariosFinalesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Miembros
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .Where(x => x.Estado == EstadoMiembro.Activo)
            .OrderBy(x => x.Apellidos)
            .ThenBy(x => x.Nombre)
            .Select(x => new BeneficiarioFinalDto(
                "NO_DEFINIDO",
                x.Documento,
                x.Nombre,
                x.Apellidos,
                "CO",
                x.TipoAfiliacion.ToString()))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ReporteExogenaDto>> GetReporteExogenaAsync(int anio, int? mes, CancellationToken cancellationToken = default)
    {
        var agregados = await _context.AsientosContables
            .AsNoTracking()
            .Join(
                _context.Comprobantes.AsNoTracking(),
                asiento => asiento.ComprobanteId,
                comprobante => comprobante.Id,
                (asiento, comprobante) => new { asiento, comprobante })
            .Where(x => x.comprobante.EstadoComprobante == EstadoComprobante.Asentado)
            .Where(x => x.comprobante.Fecha.Year == anio)
            .Where(x => !mes.HasValue || x.comprobante.Fecha.Month == mes.Value)
            .Join(
                _context.CuentasContables.AsNoTracking(),
                item => item.asiento.CuentaContableId,
                cuenta => cuenta.Id,
                (item, cuenta) => new { item.asiento, cuenta })
            .GroupBy(x => new
            {
                x.asiento.TerceroId,
                CuentaContableCodigo = x.cuenta.Codigo,
                CuentaContableNombre = x.cuenta.Descripcion
            })
            .Select(x => new ExogenaAggregateRow(
                x.Key.TerceroId,
                x.Key.CuentaContableCodigo,
                x.Key.CuentaContableNombre,
                x.Sum(item => item.asiento.Debe),
                x.Sum(item => item.asiento.Haber)))
            .ToListAsync(cancellationToken);

        var tercerosIds = agregados
            .Where(x => x.TerceroId.HasValue)
            .Select(x => x.TerceroId!.Value)
            .Distinct()
            .ToArray();

        var miembros = await _context.Miembros
            .AsNoTracking()
            .Where(x => tercerosIds.Contains(x.Id))
            .Select(x => new { x.Id, x.Documento, x.Nombre, x.Apellidos })
            .ToListAsync(cancellationToken);

        var donantes = await _context.Donantes
            .AsNoTracking()
            .Where(x => tercerosIds.Contains(x.Id))
            .Select(x => new { x.Id, x.NumeroDocumento, x.NombreORazonSocial })
            .ToListAsync(cancellationToken);

        var miembrosPorId = miembros.ToDictionary(
            x => x.Id,
            x => new TerceroInfo(x.Documento, $"{x.Nombre} {x.Apellidos}".Trim()));

        var donantesPorId = donantes.ToDictionary(
            x => x.Id,
            x => new TerceroInfo(x.NumeroDocumento, x.NombreORazonSocial));

        return agregados
            .Select(x =>
            {
                var info = ResolverTerceroInfo(x.TerceroId, miembrosPorId, donantesPorId);
                var saldoMovimiento = x.TotalDebito - x.TotalCredito;

                return new ReporteExogenaDto(
                    info.Documento,
                    info.Nombre,
                    x.CuentaContableCodigo,
                    x.CuentaContableNombre,
                    x.TotalDebito,
                    x.TotalCredito,
                    saldoMovimiento);
            })
            .OrderBy(x => x.CuentaContableCodigo)
            .ThenBy(x => x.TerceroId)
            .ToList();
    }

    private static TerceroInfo ResolverTerceroInfo(
        Guid? terceroId,
        IReadOnlyDictionary<Guid, TerceroInfo> miembrosPorId,
        IReadOnlyDictionary<Guid, TerceroInfo> donantesPorId)
    {
        if (!terceroId.HasValue)
        {
            return new TerceroInfo("SIN_TERCERO", "Sin tercero asociado");
        }

        if (miembrosPorId.TryGetValue(terceroId.Value, out var miembroInfo))
        {
            return miembroInfo;
        }

        if (donantesPorId.TryGetValue(terceroId.Value, out var donanteInfo))
        {
            return donanteInfo;
        }

        return new TerceroInfo(terceroId.Value.ToString(), "Tercero sin homologación");
    }

    private sealed record ExogenaAggregateRow(
        Guid? TerceroId,
        string CuentaContableCodigo,
        string CuentaContableNombre,
        decimal TotalDebito,
        decimal TotalCredito);

    private sealed record TerceroInfo(string Documento, string Nombre);
}
