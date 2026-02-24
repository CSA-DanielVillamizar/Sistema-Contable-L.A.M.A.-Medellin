using LAMAMedellin.Domain.Enums;
using MediatR;

namespace LAMAMedellin.Application.Features.Miembros.Commands.CreateMiembro;

public sealed record CreateMiembroCommand(
    string Nombre,
    string Apellidos,
    string Documento,
    string Email,
    string Telefono,
    TipoAfiliacion TipoAfiliacion,
    EstadoMiembro Estado) : IRequest<Guid>;
