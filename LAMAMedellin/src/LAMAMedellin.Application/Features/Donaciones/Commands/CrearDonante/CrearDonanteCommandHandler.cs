using LAMAMedellin.Application.Common.Exceptions;
using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Domain.Entities;
using MediatR;

namespace LAMAMedellin.Application.Features.Donaciones.Commands.CrearDonante;

public sealed class CrearDonanteCommandHandler(IDonanteRepository donanteRepository)
    : IRequestHandler<CrearDonanteCommand, Guid>
{
    public async Task<Guid> Handle(CrearDonanteCommand request, CancellationToken cancellationToken)
    {
        var existente = await donanteRepository.GetByDocumentoAsync(
            request.TipoDocumento,
            request.NumeroDocumento,
            cancellationToken);

        if (existente is not null)
        {
            throw new ExcepcionNegocio("Ya existe un donante con el mismo tipo y número de documento.");
        }

        var donante = new Donante(
            request.NombreORazonSocial,
            request.TipoDocumento,
            request.NumeroDocumento,
            request.Email,
            request.TipoPersona);

        await donanteRepository.AddAsync(donante, cancellationToken);
        await donanteRepository.SaveChangesAsync(cancellationToken);

        return donante.Id;
    }
}
