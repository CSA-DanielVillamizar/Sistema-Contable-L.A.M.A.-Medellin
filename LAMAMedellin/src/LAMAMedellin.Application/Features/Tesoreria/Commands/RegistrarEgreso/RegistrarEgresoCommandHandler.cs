using LAMAMedellin.Application.Common.Exceptions;
using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Application.Common.Interfaces.Services;
using LAMAMedellin.Domain.Entities;
using LAMAMedellin.Domain.Enums;
using MediatR;

namespace LAMAMedellin.Application.Features.Tesoreria.Commands.RegistrarEgreso;

public sealed class RegistrarEgresoCommandHandler(
    ICajaRepository cajaRepository,
    IGeneradorConsecutivos generadorConsecutivos,
    IEgresoRepository egresoRepository,
    IComprobanteRepository comprobanteRepository,
    ICentroCostoRepository centroCostoRepository,
    ICuentaContableRepository cuentaContableRepository,
    ITransactionManager transactionManager)
    : IRequestHandler<RegistrarEgresoCommand, Guid>
{
    public async Task<Guid> Handle(RegistrarEgresoCommand request, CancellationToken cancellationToken)
    {
        return await transactionManager.ExecuteInTransactionAsync(async ct =>
        {
            var caja = await cajaRepository.GetByIdAsync(request.CajaId, ct);
            if (caja is null)
            {
                throw new ExcepcionNegocio("La caja indicada no existe.");
            }

            var centroCosto = await centroCostoRepository.GetByIdAsync(request.CentroCostoId, ct);
            if (centroCosto is null)
            {
                throw new ExcepcionNegocio("El centro de costo indicado no existe.");
            }

            var cuentaGasto = await cuentaContableRepository.GetByIdAsync(request.CuentaContableId, ct);
            if (cuentaGasto is null)
            {
                throw new ExcepcionNegocio("La cuenta contable de gasto indicada no existe.");
            }

            if (!cuentaGasto.PermiteMovimiento)
            {
                throw new ExcepcionNegocio("La cuenta contable de gasto no permite movimiento.");
            }

            var cuentaCaja = await cuentaContableRepository.GetByIdAsync(caja.CuentaContableId, ct);
            if (cuentaCaja is null)
            {
                throw new ExcepcionNegocio("La cuenta contable asociada a la caja no existe.");
            }

            if (!cuentaCaja.PermiteMovimiento)
            {
                throw new ExcepcionNegocio("La cuenta contable asociada a la caja no permite movimiento.");
            }

            var egreso = new Egreso(
                DateTime.UtcNow,
                request.Monto,
                request.Concepto,
                request.TerceroId,
                request.CuentaContableId,
                request.CajaId,
                request.CentroCostoId);

            caja.AplicarEgreso(request.Monto);

            var comprobante = new Comprobante(
                await generadorConsecutivos.SiguienteAsync(TipoComprobante.Egreso, ct),
                DateTime.UtcNow,
                TipoComprobante.Egreso,
                $"Egreso - {request.Concepto.Trim()}",
                EstadoComprobante.Asentado);

            comprobante.AgregarAsiento(AsientoContable.Crear(
                comprobante.Id,
                request.CuentaContableId,
                request.TerceroId,
                request.CentroCostoId,
                request.Monto,
                0m,
                "Registro egreso - débito"));

            comprobante.AgregarAsiento(AsientoContable.Crear(
                comprobante.Id,
                caja.CuentaContableId,
                request.TerceroId,
                request.CentroCostoId,
                0m,
                request.Monto,
                "Registro egreso - crédito"));

            egreso.AsignarComprobanteContable(comprobante.Id);

            await comprobanteRepository.AddAsync(comprobante, ct);
            await egresoRepository.AddAsync(egreso, ct);
            await egresoRepository.SaveChangesAsync(ct);

            return egreso.Id;
        }, cancellationToken);
    }
}
