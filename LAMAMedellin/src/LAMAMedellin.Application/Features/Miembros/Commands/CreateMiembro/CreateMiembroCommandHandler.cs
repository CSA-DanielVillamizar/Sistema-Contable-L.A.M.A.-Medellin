using LAMAMedellin.Application.Common.Exceptions;
using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Domain.Entities;
using MediatR;

namespace LAMAMedellin.Application.Features.Miembros.Commands.CreateMiembro;

public sealed class CreateMiembroCommandHandler(IMiembroRepository miembroRepository)
    : IRequestHandler<CreateMiembroCommand, Guid>
{
    public async Task<Guid> Handle(CreateMiembroCommand request, CancellationToken cancellationToken)
    {
        var miembroConDocumento = await miembroRepository.GetByDocumentoAsync(request.Documento, cancellationToken);
        if (miembroConDocumento is not null)
        {
            throw new ExcepcionNegocio("Ya existe un miembro con el mismo documento.");
        }

        var miembroConEmail = await miembroRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (miembroConEmail is not null)
        {
            throw new ExcepcionNegocio("Ya existe un miembro con el mismo correo.");
        }

        var miembro = new Miembro(
            request.Nombre,
            request.Apellidos,
            request.Documento,
            request.Email,
            request.Telefono,
            request.TipoAfiliacion,
            request.Estado);

        await miembroRepository.AddAsync(miembro, cancellationToken);
        await miembroRepository.SaveChangesAsync(cancellationToken);

        return miembro.Id;
    }
}
