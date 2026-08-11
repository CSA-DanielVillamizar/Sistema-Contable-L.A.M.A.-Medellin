using LAMAMedellin.Domain.Enums;
using MediatR;

namespace LAMAMedellin.Application.Features.Miembros.Commands.UpdateMiembro;

public sealed record UpdateMiembroCommand(
    Guid Id,
    GrupoSanguineo TipoSangre,
    string NombreContactoEmergencia,
    string TelefonoContactoEmergencia,
    string MarcaMoto,
    string ModeloMoto,
    int Cilindraje,
    string Placa,
    TipoAfiliacion TipoAfiliacion,
    RangoClub? Rango,
    bool EsActivo) : IRequest<Unit>;
