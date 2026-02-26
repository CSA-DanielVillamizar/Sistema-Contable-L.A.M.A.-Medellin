using MediatR;

namespace LAMAMedellin.Application.Features.Donaciones.Queries.GetCertificadoDonacion;

public sealed record GetCertificadoDonacionQuery(Guid DonacionId) : IRequest<CertificadoDonacionDto?>;
