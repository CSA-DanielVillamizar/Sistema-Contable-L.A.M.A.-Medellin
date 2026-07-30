using LAMAMedellin.Application.Common.Exceptions;
using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Application.Features.Contabilidad.Commands.RegistrarComprobante;
using LAMAMedellin.Application.Features.Tesoreria.Commands.RegistrarEgreso;
using LAMAMedellin.Domain.Enums;
using MediatR;

namespace LAMAMedellin.Application.Features.CuentasPorPagar.Commands.PagarCuentaPorPagar;

public sealed class PagarCuentaPorPagarCommandHandler(
    ICuentaPorPagarRepository cuentaPorPagarRepository,
    IMapeoContableRepository mapeoRepository,
    ISender sender)
    : IRequestHandler<PagarCuentaPorPagarCommand>
{
    public async Task Handle(PagarCuentaPorPagarCommand request, CancellationToken cancellationToken)
    {
        var cuenta = await cuentaPorPagarRepository.GetByIdAsync(request.CuentaPorPagarId, cancellationToken)
            ?? throw new ExcepcionNegocio("La cuenta por pagar indicada no existe.");

        // Se aplica primero contra la obligacion: si el monto excede el saldo,
        // la regla salta aqui y no llega a moverse dinero del banco.
        cuenta.AplicarPago(request.Monto);

        await sender.Send(new RegistrarEgresoCommand(
            request.Monto,
            $"Pago factura {cuenta.NumeroFactura} - {cuenta.NombreProveedor}",
            null,
            cuenta.CuentaContableGastoId,
            request.BancoId,
            cuenta.CentroCostoId,
            request.MedioPago), cancellationToken);

        await RegistrarDiferenciaEnCambioAsync(cuenta, request, cancellationToken);

        await cuentaPorPagarRepository.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Diferencia en cambio (historia 1-17).
    ///
    /// Solo aplica a obligaciones pactadas en USD y solo cuando la tasa de
    /// liquidacion difiere de la reconocida. El asiento va contra las cuentas
    /// que el contador mapeo para ganancia y perdida, no contra codigos fijos
    /// en el codigo, que es justamente lo que resuelve la historia 1-2.
    /// </summary>
    private async Task RegistrarDiferenciaEnCambioAsync(
        Domain.Entities.CuentaPorPagar cuenta,
        PagarCuentaPorPagarCommand request,
        CancellationToken cancellationToken)
    {
        if (!cuenta.EsEnMonedaExtranjera || request.MontoUSD is null || request.TasaCambioLiquidacion is null)
        {
            return;
        }

        var diferencia = cuenta.CalcularDiferenciaEnCambio(
            request.MontoUSD.Value,
            request.TasaCambioLiquidacion.Value);

        if (diferencia == 0m)
        {
            return;
        }

        var esPerdida = diferencia > 0m;
        var operacion = esPerdida
            ? TipoOperacionContable.GastoDiferenciaCambio
            : TipoOperacionContable.IngresoDiferenciaCambio;

        var mapeo = await mapeoRepository.GetPorOperacionAsync(operacion, cancellationToken)
            ?? throw new ExcepcionNegocio(
                $"No hay cuenta configurada para '{operacion}'. Definala en el mapeo contable antes de liquidar en USD.");

        var contrapartida = await mapeoRepository.GetPorOperacionAsync(
                TipoOperacionContable.GastoBancario, cancellationToken)
            ?? throw new ExcepcionNegocio(
                "No hay cuenta configurada para la contrapartida de la diferencia en cambio.");

        var valor = Math.Abs(diferencia);

        // Se registra como Ajuste y no como Egreso: no hay dinero saliendo, es
        // el reconocimiento de que el pasivo valia otra cosa en pesos. El tipo
        // Ajuste ademas es el unico que el cierre de periodo deja pasar.
        await sender.Send(new RegistrarComprobanteCommand(
            DateTime.UtcNow,
            TipoComprobante.Ajuste,
            $"Diferencia en cambio - factura {cuenta.NumeroFactura} ({cuenta.NombreProveedor})",
            [
                new RegistrarAsientoComprobanteDto(
                    esPerdida ? mapeo.CuentaContableId : contrapartida.CuentaContableId,
                    null,
                    cuenta.CentroCostoId,
                    valor,
                    0m,
                    $"Liquidacion USD factura {cuenta.NumeroFactura}"),
                new RegistrarAsientoComprobanteDto(
                    esPerdida ? contrapartida.CuentaContableId : mapeo.CuentaContableId,
                    null,
                    cuenta.CentroCostoId,
                    0m,
                    valor,
                    $"Liquidacion USD factura {cuenta.NumeroFactura}"),
            ]), cancellationToken);
    }
}
