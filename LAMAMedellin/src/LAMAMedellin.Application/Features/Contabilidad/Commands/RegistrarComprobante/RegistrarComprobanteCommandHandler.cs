using LAMAMedellin.Application.Common.Exceptions;
using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Application.Common.Interfaces.Services;
using LAMAMedellin.Domain.Entities;
using LAMAMedellin.Domain.Enums;
using MediatR;

namespace LAMAMedellin.Application.Features.Contabilidad.Commands.RegistrarComprobante;

public sealed class RegistrarComprobanteCommandHandler(
    IComprobanteRepository comprobanteRepository,
    IGeneradorConsecutivos generadorConsecutivos,
    ICuentaContableRepository cuentaContableRepository,
    ICentroCostoRepository centroCostoRepository,
    IMiembroRepository miembroRepository,
    IDonanteRepository donanteRepository)
    : IRequestHandler<RegistrarComprobanteCommand, Guid>
{
    public async Task<Guid> Handle(RegistrarComprobanteCommand request, CancellationToken cancellationToken)
    {
        var cuentasPorId = await CargarYValidarCuentasAsync(request, cancellationToken);
        await ValidarCentrosCostoAsync(request, cancellationToken);
        await ValidarTercerosAsync(request, cancellationToken);

        var comprobante = new Comprobante(
            await generadorConsecutivos.SiguienteAsync(request.Tipo, cancellationToken),
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

    /// <summary>
    /// TerceroId es un Guid suelto: no tiene clave foranea porque un tercero
    /// puede ser un miembro o un donante, y no existe una tabla unica que los
    /// reuna. Sin esta comprobacion se podia guardar un identificador que no
    /// corresponde a nadie, y el asiento quedaba con un tercero fantasma que
    /// solo se descubria al preparar la exogena.
    /// </summary>
    private async Task ValidarTercerosAsync(RegistrarComprobanteCommand request, CancellationToken cancellationToken)
    {
        var idsTerceros = request.Asientos
            .Select(x => x.TerceroId)
            .Where(x => x.HasValue)
            .Select(x => x!.Value)
            .Distinct()
            .ToArray();

        foreach (var terceroId in idsTerceros)
        {
            var esMiembro = await miembroRepository.GetByIdAsync(terceroId, cancellationToken) is not null;

            if (esMiembro)
            {
                continue;
            }

            var esDonante = await donanteRepository.GetByIdAsync(terceroId, cancellationToken) is not null;

            if (!esDonante)
            {
                throw new ExcepcionNegocio(
                    $"El tercero {terceroId} no corresponde a ningun miembro ni donante registrado.");
            }
        }
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
}
