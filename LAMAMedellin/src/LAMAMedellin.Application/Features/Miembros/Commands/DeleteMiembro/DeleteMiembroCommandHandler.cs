using LAMAMedellin.Application.Common.Exceptions;
using LAMAMedellin.Application.Common.Interfaces.Repositories;
using MediatR;

namespace LAMAMedellin.Application.Features.Miembros.Commands.DeleteMiembro;

public sealed class DeleteMiembroCommandHandler(IMiembroRepository miembroRepository)
    : IRequestHandler<DeleteMiembroCommand, Unit>
{
    public async Task<Unit> Handle(DeleteMiembroCommand request, CancellationToken cancellationToken)
    {
        var miembro = await miembroRepository.GetByIdAsync(request.Id, cancellationToken);
        if (miembro is null)
        {
            throw new ExcepcionNegocio("El miembro indicado no existe.");
        }

        miembro.Desactivar();
        await miembroRepository.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
