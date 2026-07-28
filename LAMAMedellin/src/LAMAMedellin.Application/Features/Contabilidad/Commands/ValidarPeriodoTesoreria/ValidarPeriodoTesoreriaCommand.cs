using MediatR;

namespace LAMAMedellin.Application.Features.Contabilidad.Commands.ValidarPeriodoTesoreria;

public sealed record ValidarPeriodoTesoreriaCommand(int Anio, int Mes) : IRequest<Unit>;
