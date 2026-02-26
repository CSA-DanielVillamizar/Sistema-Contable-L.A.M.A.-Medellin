using LAMAMedellin.Application.Common.Exceptions;
using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Domain.Entities;
using LAMAMedellin.Domain.Enums;
using MediatR;

namespace LAMAMedellin.Application.Features.Donaciones.Commands.RegistrarDonacion;

public sealed class RegistrarDonacionCommandHandler(
    IDonanteRepository donanteRepository,
    IDonacionRepository donacionRepository,
    IBancoRepository bancoRepository,
    ICentroCostoRepository centroCostoRepository,
    ITransaccionRepository transaccionRepository)
    : IRequestHandler<RegistrarDonacionCommand, Guid>
{
    public async Task<Guid> Handle(RegistrarDonacionCommand request, CancellationToken cancellationToken)
    {
        var donante = await donanteRepository.GetByIdAsync(request.DonanteId, cancellationToken);
        if (donante is null)
        {
            throw new ExcepcionNegocio("El donante indicado no existe.");
        }

        var banco = await bancoRepository.GetByIdAsync(request.BancoId, cancellationToken);
        if (banco is null)
        {
            throw new ExcepcionNegocio("El banco indicado no existe.");
        }

        var centroCosto = await centroCostoRepository.GetByIdAsync(request.CentroCostoId, cancellationToken);
        if (centroCosto is null)
        {
            throw new ExcepcionNegocio("El centro de costo indicado no existe.");
        }

        var codigoVerificacion = await GenerarCodigoUnicoAsync(donacionRepository, cancellationToken);

        var donacion = new Donacion(
            request.DonanteId,
            request.MontoCOP,
            DateTime.UtcNow,
            request.BancoId,
            request.CentroCostoId,
            codigoVerificacion,
            request.FormaDonacion,
            request.MedioPagoODescripcion);

        banco.AplicarIngreso(request.MontoCOP);

        var descripcion = string.IsNullOrWhiteSpace(request.Descripcion)
            ? $"Donación {codigoVerificacion} - {donante.NombreORazonSocial}"
            : request.Descripcion.Trim();

        var transaccion = new Transaccion(
            request.MontoCOP,
            DateTime.UtcNow,
            TipoTransaccion.Ingreso,
            request.MedioPago,
            request.CentroCostoId,
            request.BancoId,
            descripcion);

        await donacionRepository.AddAsync(donacion, cancellationToken);
        await transaccionRepository.AddAsync(transaccion, cancellationToken);
        await donacionRepository.SaveChangesAsync(cancellationToken);

        return donacion.Id;
    }

    private static async Task<string> GenerarCodigoUnicoAsync(IDonacionRepository donacionRepository, CancellationToken cancellationToken)
    {
        for (var intento = 0; intento < 5; intento++)
        {
            var codigo = Guid.NewGuid().ToString("N")[..16].ToUpperInvariant();
            var existe = await donacionRepository.ExisteCodigoVerificacionAsync(codigo, cancellationToken);
            if (!existe)
            {
                return codigo;
            }
        }

        throw new ExcepcionNegocio("No fue posible generar un código de verificación único.");
    }
}
