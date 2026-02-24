using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LAMAMedellin.Infrastructure.Persistence.Repositories;

public sealed class CuotaAsambleaRepository(LamaDbContext dbContext) : ICuotaAsambleaRepository
{
    public Task<CuotaAsamblea?> GetVigentePorPeriodoAsync(int anio, int mes, CancellationToken cancellationToken = default)
    {
        return dbContext.CuotasAsamblea
            .Where(c => c.Anio < anio || (c.Anio == anio && c.MesInicioCobro <= mes))
            .OrderByDescending(c => c.Anio)
            .ThenByDescending(c => c.MesInicioCobro)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
