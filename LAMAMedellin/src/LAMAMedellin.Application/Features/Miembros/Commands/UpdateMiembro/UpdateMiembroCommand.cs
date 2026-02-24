using LAMAMedellin.Domain.Enums;
using MediatR;

namespace LAMAMedellin.Application.Features.Miembros.Commands.UpdateMiembro;

public sealed record UpdateMiembroCommand(
    Guid Id,
    string Nombre,
    string Apellidos,
    string Documento,
    string Email,
    string Telefono,
    TipoAfiliacion TipoAfiliacion,
    EstadoMiembro Estado) : IRequest<Unit>;
