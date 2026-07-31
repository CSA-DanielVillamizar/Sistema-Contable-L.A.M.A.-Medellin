using LAMAMedellin.Application.Common.Exceptions;
using LAMAMedellin.Application.Common.Interfaces.Repositories;
using MediatR;

namespace LAMAMedellin.Application.Features.Configuracion.Commands.ActualizarCentroCosto;

public sealed class ActualizarCentroCostoCommandHandler(ICentroCostoRepository centroCostoRepository)
    : IRequestHandler<ActualizarCentroCostoCommand>
{
    public async Task Handle(ActualizarCentroCostoCommand request, CancellationToken cancellationToken)
    {
        var todos = await centroCostoRepository.GetAllAsync(cancellationToken);

        var centroCosto = todos.FirstOrDefault(x => x.Id == request.Id)
            ?? throw new ExcepcionNegocio($"El centro de costo con Id {request.Id} no existe.");

        var nombre = request.Nombre.Trim();

        if (todos.Any(x => x.Id != request.Id && string.Equals(x.Nombre, nombre, StringComparison.OrdinalIgnoreCase)))
        {
            throw new ExcepcionNegocio($"Ya existe otro centro de costo llamado '{nombre}'.");
        }

        centroCosto.ActualizarNombre(nombre);
        centroCosto.ActualizarTipo(request.Tipo);

        await centroCostoRepository.SaveChangesAsync(cancellationToken);
    }
}
