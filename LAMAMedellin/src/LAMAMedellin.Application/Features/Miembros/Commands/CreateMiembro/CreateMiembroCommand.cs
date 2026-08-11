using LAMAMedellin.Domain.Enums;
using MediatR;

namespace LAMAMedellin.Application.Features.Miembros.Commands.CreateMiembro;

public sealed record CreateMiembroCommand(
    string DocumentoIdentidad,
    string Nombres,
    string Apellidos,
    string Apodo,
    DateOnly FechaIngreso,
    GrupoSanguineo TipoSangre,
    string NombreContactoEmergencia,
    string TelefonoContactoEmergencia,
    string MarcaMoto,
    string ModeloMoto,
    int Cilindraje,
    string Placa,
    TipoAfiliacion TipoAfiliacion,
    RangoClub? Rango) : IRequest<Guid>;
