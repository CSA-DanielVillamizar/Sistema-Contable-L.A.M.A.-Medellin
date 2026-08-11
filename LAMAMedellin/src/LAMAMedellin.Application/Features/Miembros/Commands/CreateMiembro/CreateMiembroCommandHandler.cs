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
        var miembroConDocumento = await miembroRepository.GetByDocumentoAsync(request.DocumentoIdentidad, cancellationToken);
        if (miembroConDocumento is not null)
        {
            throw new ExcepcionNegocio("Ya existe un miembro con el mismo documento.");
        }

        var miembro = new Miembro(
            request.DocumentoIdentidad,
            request.Nombres,
            request.Apellidos,
            request.Apodo,
            request.FechaIngreso,
            request.TipoSangre,
            request.NombreContactoEmergencia,
            request.TelefonoContactoEmergencia,
            request.MarcaMoto,
            request.ModeloMoto,
            request.Cilindraje,
            request.Placa,
            request.Rango,
            request.TipoAfiliacion);

        await miembroRepository.AddAsync(miembro, cancellationToken);
        await miembroRepository.SaveChangesAsync(cancellationToken);

        return miembro.Id;
    }
}
