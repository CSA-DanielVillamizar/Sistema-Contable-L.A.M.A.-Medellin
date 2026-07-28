using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Application.Common.Interfaces.Services;
using LAMAMedellin.Domain.Entities;
using MediatR;

namespace LAMAMedellin.Application.Features.Contabilidad.Commands.ValidarPeriodoTesoreria;

/// <summary>
/// Marca del Tesorero previa al cierre (historia 1-5). Si el periodo aun no
/// existe se crea abierto y se valida en el acto: los periodos no se dan de
/// alta a mano, aparecen cuando alguien actua sobre ellos.
/// </summary>
public sealed class ValidarPeriodoTesoreriaCommandHandler(
    IPeriodoContableRepository periodoRepository,
    IUsuarioActual usuarioActual)
    : IRequestHandler<ValidarPeriodoTesoreriaCommand, Unit>
{
    public async Task<Unit> Handle(ValidarPeriodoTesoreriaCommand request, CancellationToken cancellationToken)
    {
        var periodo = await periodoRepository.GetPorAnioYMesAsync(request.Anio, request.Mes, cancellationToken);

        if (periodo is null)
        {
            periodo = new PeriodoContable(request.Anio, request.Mes);
            await periodoRepository.AddAsync(periodo, cancellationToken);
        }

        periodo.ValidarTesoreria(usuarioActual.Identificador);

        await periodoRepository.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
