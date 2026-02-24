using LAMAMedellin.Application.Common.Interfaces.Repositories;
using MediatR;

namespace LAMAMedellin.Application.Features.Miembros.Queries.GetMiembros;

public sealed class GetMiembrosQueryHandler(IMiembroRepository miembroRepository)
    : IRequestHandler<GetMiembrosQuery, IReadOnlyList<MiembroDto>>
{
    public async Task<IReadOnlyList<MiembroDto>> Handle(GetMiembrosQuery request, CancellationToken cancellationToken)
    {
        var miembros = await miembroRepository.GetAllAsync(cancellationToken);

        return miembros
            .Select(miembro => new MiembroDto(
                miembro.Id,
                $"{miembro.Nombre} {miembro.Apellidos}".Trim(),
                miembro.Documento,
                miembro.Email,
                miembro.Telefono,
                miembro.TipoAfiliacion.ToString(),
                miembro.Estado.ToString()))
            .ToList();
    }
}
