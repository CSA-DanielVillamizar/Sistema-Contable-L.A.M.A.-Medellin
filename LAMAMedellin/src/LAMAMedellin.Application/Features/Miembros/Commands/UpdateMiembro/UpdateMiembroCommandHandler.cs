using LAMAMedellin.Application.Common.Exceptions;
using LAMAMedellin.Application.Common.Interfaces.Repositories;
using MediatR;

namespace LAMAMedellin.Application.Features.Miembros.Commands.UpdateMiembro;

public sealed class UpdateMiembroCommandHandler(IMiembroRepository miembroRepository)
    : IRequestHandler<UpdateMiembroCommand, Unit>
{
    public async Task<Unit> Handle(UpdateMiembroCommand request, CancellationToken cancellationToken)
    {
        var miembro = await miembroRepository.GetByIdAsync(request.Id, cancellationToken);
        if (miembro is null)
        {
            throw new ExcepcionNegocio("El miembro indicado no existe.");
        }

        var miembroConDocumento = await miembroRepository.GetByDocumentoAsync(request.Documento, cancellationToken);
        if (miembroConDocumento is not null && miembroConDocumento.Id != request.Id)
        {
            throw new ExcepcionNegocio("Ya existe otro miembro con el mismo documento.");
        }

        var miembroConEmail = await miembroRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (miembroConEmail is not null && miembroConEmail.Id != request.Id)
        {
            throw new ExcepcionNegocio("Ya existe otro miembro con el mismo correo.");
        }

        miembro.ActualizarDatos(
            request.Nombre,
            request.Apellidos,
            request.Documento,
            request.Email,
            request.Telefono,
            request.TipoAfiliacion,
            request.Estado);

        await miembroRepository.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
