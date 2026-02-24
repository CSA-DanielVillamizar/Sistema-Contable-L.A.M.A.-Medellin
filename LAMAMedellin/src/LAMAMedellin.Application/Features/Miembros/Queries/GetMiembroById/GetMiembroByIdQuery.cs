using MediatR;

namespace LAMAMedellin.Application.Features.Miembros.Queries.GetMiembroById;

public sealed record GetMiembroByIdQuery(Guid Id) : IRequest<MiembroDto?>;
