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
                miembro.DocumentoIdentidad,
                miembro.Nombres,
                miembro.Apellidos,
                miembro.Apodo,
                miembro.FechaIngreso,
                miembro.Rango?.ToString(),
                miembro.TipoAfiliacion.ToString(),
                miembro.EsActivo,
                miembro.TipoSangre.ToString(),
                miembro.NombreContactoEmergencia,
                miembro.TelefonoContactoEmergencia,
                miembro.MarcaMoto,
                miembro.ModeloMoto,
                miembro.Placa,
                miembro.Cilindraje))
            .ToList();
    }
}
