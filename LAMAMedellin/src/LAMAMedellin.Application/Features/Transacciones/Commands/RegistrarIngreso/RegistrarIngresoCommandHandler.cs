using LAMAMedellin.Application.Common.Exceptions;
using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Domain.Entities;
using LAMAMedellin.Domain.Enums;
using LAMAMedellin.Domain.ValueObjects;
using MediatR;

namespace LAMAMedellin.Application.Features.Transacciones.Commands.RegistrarIngreso;

public sealed class RegistrarIngresoCommandHandler(
    ITransaccionRepository transaccionRepository,
    IBancoRepository bancoRepository,
    ICentroCostoRepository centroCostoRepository)
    : IRequestHandler<RegistrarIngresoCommand, Guid>
{
    public async Task<Guid> Handle(RegistrarIngresoCommand request, CancellationToken cancellationToken)
    {
        var banco = await bancoRepository.GetByIdAsync(request.BancoId, cancellationToken);
        if (banco is null)
        {
            throw new ExcepcionNegocio("El banco indicado no existe.");
        }

        var centroCosto = await centroCostoRepository.GetByIdAsync(request.CentroCostoId, cancellationToken);
        if (centroCosto is null)
        {
            throw new ExcepcionNegocio("El centro de costo indicado no existe.");
        }

        TransaccionMultimoneda? transaccionMultimoneda = null;
        if (EsUsd(request.MonedaOrigen))
        {
            transaccionMultimoneda = new TransaccionMultimoneda(
                request.MonedaOrigen!,
                request.MontoMonedaOrigen ?? throw new ExcepcionNegocio("MontoMonedaOrigen es obligatorio cuando MonedaOrigen es USD."),
                request.TasaCambioUsada ?? throw new ExcepcionNegocio("TasaCambioUsada es obligatoria cuando MonedaOrigen es USD."),
                request.FechaTasaCambio ?? throw new ExcepcionNegocio("FechaTasaCambio es obligatoria cuando MonedaOrigen es USD."),
                request.FuenteTasaCambio ?? throw new ExcepcionNegocio("FuenteTasaCambio es obligatoria cuando MonedaOrigen es USD."));
        }

        var transaccion = new Transaccion(
            request.MontoCOP,
            DateTime.UtcNow,
            TipoTransaccion.Ingreso,
            request.MedioPago,
            request.CentroCostoId,
            request.BancoId,
            request.Descripcion,
            transaccionMultimoneda);

        transaccion.VincularBanco(banco);
        transaccion.VincularCentroCosto(centroCosto);

        banco.AplicarIngreso(request.MontoCOP);

        await transaccionRepository.AddAsync(transaccion, cancellationToken);
        await transaccionRepository.SaveChangesAsync(cancellationToken);

        return transaccion.Id;
    }

    private static bool EsUsd(string? monedaOrigen) =>
        string.Equals(monedaOrigen, "USD", StringComparison.OrdinalIgnoreCase);
}
