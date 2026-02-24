using MediatR;

namespace LAMAMedellin.Application.Features.Cartera.Queries.GetCarteraPendiente;

public sealed record GetCarteraPendienteQuery : IRequest<List<CarteraPendienteDto>>;
