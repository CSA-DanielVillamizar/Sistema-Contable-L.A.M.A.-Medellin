using LAMAMedellin.Application.Common.Exceptions;
using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Domain.Entities;
using LAMAMedellin.Domain.Enums;
using MediatR;

namespace LAMAMedellin.Application.Features.Cartera.Commands.GenerarRenovacionAnual;

/// <summary>
/// Genera la obligacion anual de renovacion de membresia internacional
/// L.A.M.A. para cada miembro activo (historia fx-02 / capitulo Medellin).
///
/// A diferencia de la cuota mensual, aqui no hay exentos: Full Color,
/// Rockets, Prospecto, Lady L.A.M.A., Spousal y Youth pagan USD 20; Asociado
/// paga USD 40. El capitulo la recauda en diciembre, asi que se representa
/// con el periodo YYYY-12 para reutilizar la misma idempotencia por
/// miembro+concepto+periodo que ya usa la cartera mensual.
///
/// La tasa de cambio la confirma quien ejecuta la generacion (igual que en
/// ingresos/egresos en USD): no se consulta ninguna fuente automatica desde
/// el backend, para no depender de una llamada externa en un proceso que
/// corre una vez al año.
/// </summary>
public sealed class GenerarRenovacionAnualCommandHandler(
    IMiembroRepository miembroRepository,
    IConceptoCobroRepository conceptoCobroRepository,
    ICuentaPorCobrarRepository cuentaPorCobrarRepository)
    : IRequestHandler<GenerarRenovacionAnualCommand, int>
{
    private const decimal ValorUsdEstandar = 20m;
    private const decimal ValorUsdAsociado = 40m;

    public async Task<int> Handle(GenerarRenovacionAnualCommand request, CancellationToken cancellationToken)
    {
        var concepto = await conceptoCobroRepository.GetRenovacionAnualAsync(cancellationToken)
            ?? throw new ExcepcionNegocio(
                "No existe el concepto de cobro de renovacion de membresia internacional. " +
                "Configure la cartera antes de generar.");

        var miembrosActivos = await miembroRepository.GetActivosAsync(cancellationToken);

        var periodo = $"{request.Anio}-12";
        var inicio = new DateOnly(request.Anio, 12, 1);
        var fin = inicio.AddMonths(1).AddDays(-1);

        var nuevas = new List<CuentaPorCobrar>();

        foreach (var miembro in miembrosActivos)
        {
            // Idempotencia real: se consulta por miembro, concepto Y periodo,
            // igual que la generacion mensual. Volver a ejecutar el mismo
            // anio no duplica nada.
            var yaExiste = await cuentaPorCobrarRepository.ExisteParaMiembroYPeriodoAsync(
                miembro.Id,
                concepto.Id,
                periodo,
                cancellationToken);

            if (yaExiste)
            {
                continue;
            }

            var valorUsd = miembro.TipoAfiliacion == TipoAfiliacion.Asociado ? ValorUsdAsociado : ValorUsdEstandar;
            var valorCop = Math.Round(valorUsd * request.TasaCambioUsada, 0, MidpointRounding.AwayFromZero);

            nuevas.Add(new CuentaPorCobrar(
                miembro.Id,
                concepto.Id,
                periodo,
                inicio,
                fin,
                valorCop));
        }

        if (nuevas.Count == 0)
        {
            return 0;
        }

        await cuentaPorCobrarRepository.AddRangeAsync(nuevas, cancellationToken);
        await cuentaPorCobrarRepository.SaveChangesAsync(cancellationToken);

        return nuevas.Count;
    }
}
