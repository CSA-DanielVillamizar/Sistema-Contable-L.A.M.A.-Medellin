namespace LAMAMedellin.Application.Features.Tributario.Queries.GetReporteBeneficiariosFinales;

public sealed record BeneficiarioFinalDto(
    string TipoDocumento,
    string NumeroDocumento,
    string Nombres,
    string Apellidos,
    string PaisResidencia,
    string CargoORol);
