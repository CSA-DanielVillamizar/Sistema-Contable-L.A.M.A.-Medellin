using LAMAMedellin.Domain.Enums;
using MediatR;

namespace LAMAMedellin.Application.Features.Donaciones.Commands.CrearDonante;

public sealed record CrearDonanteCommand(
    string NombreORazonSocial,
    TipoDocumentoDonante TipoDocumento,
    string NumeroDocumento,
    string Email,
    TipoPersonaDonante TipoPersona) : IRequest<Guid>;
