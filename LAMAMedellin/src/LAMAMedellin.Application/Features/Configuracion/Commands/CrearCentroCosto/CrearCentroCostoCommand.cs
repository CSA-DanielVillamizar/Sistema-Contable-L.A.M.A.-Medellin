using LAMAMedellin.Domain.Enums;
using MediatR;

namespace LAMAMedellin.Application.Features.Configuracion.Commands.CrearCentroCosto;

/// <summary>
/// Alta de centro de costo (historia 0-5 del backlog). Hasta ahora solo los
/// creaba el seeder, asi que el capitulo no podia abrir un centro nuevo para
/// una actividad y todos sus movimientos terminaban imputados al general.
/// </summary>
public sealed record CrearCentroCostoCommand(string Nombre, TipoCentroCosto Tipo) : IRequest<Guid>;
