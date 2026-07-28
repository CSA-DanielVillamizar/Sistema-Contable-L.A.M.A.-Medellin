using LAMAMedellin.Application.Common.Exceptions;
using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Application.Common.Interfaces.Services;
using LAMAMedellin.Domain.Entities;
using LAMAMedellin.Domain.Enums;
using MediatR;

namespace LAMAMedellin.Application.Features.Tesoreria.Commands.RegistrarIngreso;

public sealed class RegistrarIngresoCommandHandler(
    ICajaRepository cajaRepository,
    IGeneradorConsecutivos generadorConsecutivos,
    IIngresoRepository ingresoRepository,
    IComprobanteRepository comprobanteRepository,
    ICentroCostoRepository centroCostoRepository,
    ICuentaContableRepository cuentaContableRepository,
    ITransactionManager transactionManager)
    : IRequestHandler<RegistrarIngresoCommand, Guid>
{
    public async Task<Guid> Handle(RegistrarIngresoCommand request, CancellationToken cancellationToken)
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

            var cuentaIngreso = await cuentaContableRepository.GetByIdAsync(request.CuentaContableId, ct);
            if (cuentaIngreso is null)
            {
                throw new ExcepcionNegocio("La cuenta contable de ingreso indicada no existe.");
            }

            if (!cuentaIngreso.PermiteMovimiento)
            {
                throw new ExcepcionNegocio("La cuenta contable de ingreso no permite movimiento.");
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

            var ingreso = new Ingreso(
                DateTime.UtcNow,
                request.Monto,
                request.Concepto,
                request.TerceroId,
                request.CuentaContableId,
                request.CajaId,
                request.CentroCostoId);

            caja.AplicarIngreso(request.Monto);

            var comprobante = new Comprobante(
                await generadorConsecutivos.SiguienteAsync(TipoComprobante.Ingreso, ct),
                DateTime.UtcNow,
                TipoComprobante.Ingreso,
                $"Ingreso - {request.Concepto.Trim()}",
                EstadoComprobante.Asentado);

            comprobante.AgregarAsiento(AsientoContable.Crear(
                comprobante.Id,
                caja.CuentaContableId,
                request.TerceroId,
                request.CentroCostoId,
                request.Monto,
                0m,
                "Registro ingreso - debito"));

            comprobante.AgregarAsiento(AsientoContable.Crear(
                comprobante.Id,
                request.CuentaContableId,
                request.TerceroId,
                request.CentroCostoId,
                0m,
                request.Monto,
                "Registro ingreso - credito"));

            ingreso.AsignarComprobanteContable(comprobante.Id);

            await ingresoRepository.AddAsync(ingreso, ct);
            await comprobanteRepository.AddAsync(comprobante, ct);
            await ingresoRepository.SaveChangesAsync(ct);

            return ingreso.Id;
        }, cancellationToken);
    }
}
