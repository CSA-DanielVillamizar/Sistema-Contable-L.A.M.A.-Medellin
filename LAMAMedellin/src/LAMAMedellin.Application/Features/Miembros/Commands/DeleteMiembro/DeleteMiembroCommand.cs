using MediatR;

namespace LAMAMedellin.Application.Features.Miembros.Commands.DeleteMiembro;

public sealed record DeleteMiembroCommand(Guid Id) : IRequest<Unit>;
