using LAMAMedellin.Application.Common.Exceptions;
using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Domain.Entities;
using MediatR;

namespace LAMAMedellin.Application.Features.Cartera.Commands.GenerarCarteraMensual;

public sealed class GenerarCarteraMensualCommandHandler(
    IMiembroRepository miembroRepository,
    ICuotaAsambleaRepository cuotaAsambleaRepository,
    ICuentaPorCobrarRepository cuentaPorCobrarRepository)
    : IRequestHandler<GenerarCarteraMensualCommand, int>
{
    public async Task<int> Handle(GenerarCarteraMensualCommand request, CancellationToken cancellationToken)
    {
        var anio = int.Parse(request.Periodo.AsSpan(0, 4));
        var mes = int.Parse(request.Periodo.AsSpan(5, 2));

        var cuotaAsamblea = await cuotaAsambleaRepository.GetVigentePorPeriodoAsync(anio, mes, cancellationToken);
        if (cuotaAsamblea is null)
        {
            throw new ExcepcionNegocio($"No existe CuotaAsamblea vigente para el periodo {request.Periodo}.");
        }

        var miembrosActivos = await miembroRepository.GetActivosAsync(cancellationToken);
        var cuentasNuevas = new List<CuentaPorCobrar>();

        foreach (var miembro in miembrosActivos)
        {
            var existeCuenta = await cuentaPorCobrarRepository.ExistePorMiembroYPeriodoAsync(
                miembro.Id,
                request.Periodo,
                cancellationToken);

            if (existeCuenta)
            {
                continue;
            }

            cuentasNuevas.Add(new CuentaPorCobrar(
                miembro.Id,
                request.Periodo,
                cuotaAsamblea.ValorMensualCOP));
        }

        if (cuentasNuevas.Count == 0)
        {
            return 0;
        }

        await cuentaPorCobrarRepository.AddRangeAsync(cuentasNuevas, cancellationToken);
        await cuentaPorCobrarRepository.SaveChangesAsync(cancellationToken);

        return cuentasNuevas.Count;
    }
}
