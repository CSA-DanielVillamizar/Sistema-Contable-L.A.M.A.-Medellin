using MediatR;

namespace LAMAMedellin.Application.Features.Tesoreria.Commands.CambiarEstadoCuentaBancaria;

/// <summary>
/// Da de baja o reactiva una cuenta. Se prefiere sobre el borrado porque los
/// movimientos ya registrados contra ella deben seguir existiendo: una cuenta
/// inactiva conserva su historia pero desaparece de los desplegables.
/// </summary>
public sealed record CambiarEstadoCuentaBancariaCommand(Guid Id, bool EsActivo) : IRequest;
