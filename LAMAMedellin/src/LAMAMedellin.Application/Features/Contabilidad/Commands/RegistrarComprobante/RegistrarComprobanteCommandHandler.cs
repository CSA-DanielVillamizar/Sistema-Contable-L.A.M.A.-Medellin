using LAMAMedellin.Application.Common.Exceptions;
using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Domain.Entities;
using LAMAMedellin.Domain.Enums;
using MediatR;

namespace LAMAMedellin.Application.Features.Contabilidad.Commands.RegistrarComprobante;

public sealed class RegistrarComprobanteCommandHandler(
    IComprobanteRepository comprobanteRepository,
    ICuentaContableRepository cuentaContableRepository,
    ICentroCostoRepository centroCostoRepository)
    : IRequestHandler<RegistrarComprobanteCommand, Guid>
{
    public async Task<Guid> Handle(RegistrarComprobanteCommand request, CancellationToken cancellationToken)
    {
        var cuentasPorId = await CargarYValidarCuentasAsync(request, cancellationToken);
        await ValidarCentrosCostoAsync(request, cancellationToken);

        var comprobante = new Comprobante(
            GenerarNumeroConsecutivo(),
            request.Fecha,
            request.Tipo,
            request.Descripcion,
            EstadoComprobante.Borrador);

        foreach (var asiento in request.Asientos)
        {
            var cuenta = cuentasPorId[asiento.CuentaContableId];

            if (cuenta.ExigeTercero && asiento.TerceroId is null)
            {
                throw new ExcepcionNegocio("La cuenta contable exige tercero y no se envió TerceroId.");
            }

            comprobante.AgregarAsiento(AsientoContable.Crear(
                comprobante.Id,
                asiento.CuentaContableId,
                asiento.TerceroId,
                asiento.CentroCostoId,
                asiento.Debe,
                asiento.Haber,
                asiento.Referencia));
        }

        await comprobanteRepository.AddAsync(comprobante, cancellationToken);
        await comprobanteRepository.SaveChangesAsync(cancellationToken);

        return comprobante.Id;
    }

    private async Task<Dictionary<Guid, CuentaContable>> CargarYValidarCuentasAsync(
        RegistrarComprobanteCommand request,
        CancellationToken cancellationToken)
    {
        var idsCuentas = request.Asientos
            .Select(x => x.CuentaContableId)
            .Distinct()
            .ToArray();

        var cuentas = await cuentaContableRepository.GetByIdsAsync(idsCuentas, cancellationToken);
        var cuentasPorId = cuentas.ToDictionary(x => x.Id);

        foreach (var cuentaId in idsCuentas)
        {
            if (!cuentasPorId.TryGetValue(cuentaId, out var cuenta))
            {
                throw new ExcepcionNegocio("La cuenta contable indicada no existe.");
            }

            if (!cuenta.PermiteMovimiento)
            {
                throw new ExcepcionNegocio("La cuenta contable indicada no permite movimiento.");
            }
        }

        return cuentasPorId;
    }

    private async Task ValidarCentrosCostoAsync(RegistrarComprobanteCommand request, CancellationToken cancellationToken)
    {
        var idsCentrosCosto = request.Asientos
            .Select(x => x.CentroCostoId)
            .Distinct()
            .ToArray();

        foreach (var centroCostoId in idsCentrosCosto)
        {
            var centroCosto = await centroCostoRepository.GetByIdAsync(centroCostoId, cancellationToken);
            if (centroCosto is null)
            {
                throw new ExcepcionNegocio("El centro de costo indicado no existe.");
            }
        }
    }

    private static string GenerarNumeroConsecutivo() =>
        $"CMP-{DateTime.UtcNow:yyyyMMddHHmmssfff}";
}
