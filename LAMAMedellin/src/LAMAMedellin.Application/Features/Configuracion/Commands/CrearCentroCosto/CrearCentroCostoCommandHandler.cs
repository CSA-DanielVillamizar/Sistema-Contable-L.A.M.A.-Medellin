using LAMAMedellin.Application.Common.Exceptions;
using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Domain.Entities;
using MediatR;

namespace LAMAMedellin.Application.Features.Configuracion.Commands.CrearCentroCosto;

public sealed class CrearCentroCostoCommandHandler(ICentroCostoRepository centroCostoRepository)
    : IRequestHandler<CrearCentroCostoCommand, Guid>
{
    public async Task<Guid> Handle(CrearCentroCostoCommand request, CancellationToken cancellationToken)
    {
        var existentes = await centroCostoRepository.GetAllAsync(cancellationToken);

        // El nombre es lo unico que distingue un centro de otro en los informes:
        // dos con el mismo nombre vuelven ilegible cualquier reporte por centro.
        if (existentes.Any(x => string.Equals(x.Nombre, request.Nombre.Trim(), StringComparison.OrdinalIgnoreCase)))
        {
            throw new ExcepcionNegocio($"Ya existe un centro de costo llamado '{request.Nombre.Trim()}'.");
        }

        var centroCosto = new CentroCosto(request.Nombre, request.Tipo);

        await centroCostoRepository.AddAsync(centroCosto, cancellationToken);
        await centroCostoRepository.SaveChangesAsync(cancellationToken);

        return centroCosto.Id;
    }
}
