using MediatR;

namespace LAMAMedellin.Application.Features.Contabilidad.Commands.CerrarPeriodo;

public sealed record CerrarPeriodoCommand(int Anio, int Mes) : IRequest<Unit>;
