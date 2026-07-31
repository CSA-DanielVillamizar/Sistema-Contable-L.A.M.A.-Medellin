using LAMAMedellin.Domain.Enums;
using MediatR;

namespace LAMAMedellin.Application.Features.Configuracion.Commands.ActualizarCentroCosto;

public sealed record ActualizarCentroCostoCommand(Guid Id, string Nombre, TipoCentroCosto Tipo) : IRequest;
