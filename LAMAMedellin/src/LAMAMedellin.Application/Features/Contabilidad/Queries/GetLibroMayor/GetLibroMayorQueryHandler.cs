using LAMAMedellin.Application.Common.Exceptions;
using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Domain.Enums;
using MediatR;

namespace LAMAMedellin.Application.Features.Contabilidad.Queries.GetLibroMayor;

/// <summary>
/// Mayor de una cuenta: arranca del saldo acumulado hasta antes del rango y va
/// aplicando cada movimiento para mostrar el saldo corrido.
/// </summary>
public sealed class GetLibroMayorQueryHandler(ILibrosContablesRepository librosRepository)
    : IRequestHandler<GetLibroMayorQuery, LibroMayorDto>
{
    public async Task<LibroMayorDto> Handle(GetLibroMayorQuery request, CancellationToken cancellationToken)
    {
        var cuenta = await librosRepository.GetCuentaAsync(request.CuentaContableId, cancellationToken);

        if (cuenta is null)
        {
            throw new ExcepcionNegocio("La cuenta contable indicada no existe.");
        }

        var (debeAnterior, haberAnterior) = await librosRepository.GetAcumuladoAnteriorAsync(
            request.CuentaContableId,
            request.Desde,
            request.CentroCostoId,
            cancellationToken);

        var saldoAnterior = CalcularSaldo(cuenta.Naturaleza, debeAnterior, haberAnterior);

        var movimientos = await librosRepository.GetMovimientosMayorAsync(
            request.CuentaContableId,
            request.Desde,
            request.Hasta,
            request.CentroCostoId,
            cancellationToken);

        // Saldo corrido: cada linea muestra como queda la cuenta despues de
        // aplicar ese movimiento, que es lo que se espera de un mayor.
        var saldoCorrido = saldoAnterior;
        var conSaldo = new List<MovimientoLibroMayorDto>(movimientos.Count);

        foreach (var movimiento in movimientos)
        {
            saldoCorrido += CalcularSaldo(cuenta.Naturaleza, movimiento.Debe, movimiento.Haber);
            conSaldo.Add(movimiento with { SaldoAcumulado = saldoCorrido });
        }

        return new LibroMayorDto(
            cuenta.Id,
            cuenta.Codigo,
            cuenta.Descripcion,
            cuenta.Naturaleza.ToString(),
            request.Desde,
            request.Hasta,
            saldoAnterior,
            movimientos.Sum(m => m.Debe),
            movimientos.Sum(m => m.Haber),
            saldoCorrido,
            conSaldo);
    }

    /// <summary>
    /// El signo depende de la naturaleza: una cuenta debito (activo, gasto)
    /// crece con el debe; una credito (pasivo, patrimonio, ingreso) crece con el
    /// haber. Restar siempre en el mismo sentido mostraria los ingresos en
    /// negativo.
    /// </summary>
    private static decimal CalcularSaldo(NaturalezaCuenta naturaleza, decimal debe, decimal haber) =>
        naturaleza == NaturalezaCuenta.Debito ? debe - haber : haber - debe;
}
