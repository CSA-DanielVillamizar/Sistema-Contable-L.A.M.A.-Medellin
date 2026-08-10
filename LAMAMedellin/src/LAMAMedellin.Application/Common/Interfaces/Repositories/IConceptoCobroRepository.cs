using LAMAMedellin.Domain.Entities;

namespace LAMAMedellin.Application.Common.Interfaces.Repositories;

public interface IConceptoCobroRepository
{
    Task<ConceptoCobro?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<ConceptoCobro>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(ConceptoCobro conceptoCobro, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>Concepto con el que se genera la cuota mensual.</summary>
    Task<ConceptoCobro?> GetCuotaMensualAsync(CancellationToken cancellationToken = default);

    /// <summary>Concepto con el que se genera la renovacion anual de membresia internacional.</summary>
    Task<ConceptoCobro?> GetRenovacionAnualAsync(CancellationToken cancellationToken = default);
}
