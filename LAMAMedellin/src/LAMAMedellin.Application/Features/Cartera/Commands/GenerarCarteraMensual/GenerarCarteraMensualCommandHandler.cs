using LAMAMedellin.Application.Common.Exceptions;
using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Domain.Entities;
using LAMAMedellin.Domain.Enums;
using MediatR;

namespace LAMAMedellin.Application.Features.Cartera.Commands.GenerarCarteraMensual;

/// <summary>
/// Genera las obligaciones de cuota mensual del periodo (historia 1-10).
///
/// El valor sale de la cuota vigente aprobada en asamblea, no de una tarifa por
/// tipo de afiliacion: el capitulo define un unico valor en la primera asamblea
/// de cada mes y ese valor rige para todos los que pagan.
///
/// Antes este manejador construia cada cuenta por cobrar con un Guid aleatorio
/// como ConceptoCobroId, contra una clave foranea requerida, de modo que el
/// endpoint fallaba siempre que hubiera al menos un miembro activo con tarifa.
/// </summary>
public sealed class GenerarCarteraMensualCommandHandler(
    IMiembroRepository miembroRepository,
    ICuotaAsambleaRepository cuotaAsambleaRepository,
    IConceptoCobroRepository conceptoCobroRepository,
    ICuentaPorCobrarRepository cuentaPorCobrarRepository)
    : IRequestHandler<GenerarCarteraMensualCommand, int>
{
    public async Task<int> Handle(GenerarCarteraMensualCommand request, CancellationToken cancellationToken)
    {
        var (anio, mes) = ParsearPeriodo(request.Periodo);

        var cuota = await cuotaAsambleaRepository.GetVigentePorPeriodoAsync(anio, mes, cancellationToken)
            ?? throw new ExcepcionNegocio(
                $"No hay cuota de asamblea vigente para el periodo {request.Periodo}. " +
                "El tesorero debe confirmar el valor aprobado antes de generar la cartera.");

        if (cuota.ValorMensualCOP <= 0)
        {
            throw new ExcepcionNegocio(
                $"La cuota vigente para {request.Periodo} es cero. Revise el valor aprobado en asamblea.");
        }

        var concepto = await conceptoCobroRepository.GetCuotaMensualAsync(cancellationToken)
            ?? throw new ExcepcionNegocio(
                "No existe el concepto de cobro de cuota mensual. Configure la cartera antes de generar.");

        var miembrosActivos = await miembroRepository.GetActivosAsync(cancellationToken);

        var inicio = new DateOnly(anio, mes, 1);
        var fin = inicio.AddMonths(1).AddDays(-1);

        var nuevas = new List<CuentaPorCobrar>();

        foreach (var miembro in miembrosActivos)
        {
            // Spousal no paga cuota mensual. La exencion es una regla del
            // dominio y no una tarifa en cero, para que sea explicita y nadie la
            // altere sin entender lo que significa.
            if (miembro.TipoAfiliacion.ExentoDeCuotaMensual())
            {
                continue;
            }

            // Idempotencia real: se consulta por miembro, concepto Y periodo.
            // Volver a ejecutar la generacion del mismo mes no duplica nada.
            var yaExiste = await cuentaPorCobrarRepository.ExisteParaMiembroYPeriodoAsync(
                miembro.Id,
                concepto.Id,
                request.Periodo,
                cancellationToken);

            if (yaExiste)
            {
                continue;
            }

            nuevas.Add(new CuentaPorCobrar(
                miembro.Id,
                concepto.Id,
                request.Periodo,
                inicio,
                fin,
                cuota.ValorMensualCOP));
        }

        if (nuevas.Count == 0)
        {
            return 0;
        }

        await cuentaPorCobrarRepository.AddRangeAsync(nuevas, cancellationToken);
        await cuentaPorCobrarRepository.SaveChangesAsync(cancellationToken);

        return nuevas.Count;
    }

    private static (int Anio, int Mes) ParsearPeriodo(string periodo)
    {
        if (!CuentaPorCobrar.EsPeriodoValido(periodo))
        {
            throw new ExcepcionNegocio("Periodo debe tener formato YYYY-MM.");
        }

        return (int.Parse(periodo[..4]), int.Parse(periodo[5..]));
    }
}
