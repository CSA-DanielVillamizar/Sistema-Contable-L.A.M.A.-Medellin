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

        if (!request.EsActivo && miembro.EsActivo)
        {
            miembro.DarDeBaja();
        }

        if (request.EsActivo && !miembro.EsActivo)
        {
            throw new ExcepcionNegocio("No existe un flujo de reactivacion en el dominio actual de Miembro.");
        }

        miembro.AsignarRango(request.Rango);
        miembro.CambiarTipoAfiliacion(request.TipoAfiliacion);
        miembro.ActualizarDatosEmergencia(
            request.TipoSangre,
            request.NombreContactoEmergencia,
            request.TelefonoContactoEmergencia);
        miembro.ActualizarMotocicleta(
            request.MarcaMoto,
            request.ModeloMoto,
            request.Cilindraje,
            request.Placa);

        await miembroRepository.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
