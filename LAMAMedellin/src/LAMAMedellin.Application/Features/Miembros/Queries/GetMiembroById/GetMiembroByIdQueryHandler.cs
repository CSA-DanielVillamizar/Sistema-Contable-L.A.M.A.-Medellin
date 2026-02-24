using LAMAMedellin.Application.Common.Interfaces.Repositories;
using MediatR;

namespace LAMAMedellin.Application.Features.Miembros.Queries.GetMiembroById;

public sealed class GetMiembroByIdQueryHandler(IMiembroRepository miembroRepository)
    : IRequestHandler<GetMiembroByIdQuery, MiembroDto?>
{
    public async Task<MiembroDto?> Handle(GetMiembroByIdQuery request, CancellationToken cancellationToken)
    {
        var miembro = await miembroRepository.GetByIdAsync(request.Id, cancellationToken);
        if (miembro is null)
        {
            return null;
        }

        return new MiembroDto(
            miembro.Id,
            $"{miembro.Nombre} {miembro.Apellidos}".Trim(),
            miembro.Documento,
            miembro.Email,
            miembro.Telefono,
            miembro.TipoAfiliacion.ToString(),
            miembro.Estado.ToString());
    }
}
