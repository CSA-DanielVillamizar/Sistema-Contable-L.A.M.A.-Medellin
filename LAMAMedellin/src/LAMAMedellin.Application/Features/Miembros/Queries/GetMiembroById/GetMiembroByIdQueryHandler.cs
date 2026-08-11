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
            miembro.Cilindraje,
            miembro.Placa);
    }
}
