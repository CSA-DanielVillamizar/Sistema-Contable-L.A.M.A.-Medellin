using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Domain.Entities;

namespace LAMAMedellin.Infrastructure.Persistence.Repositories;

public sealed class VentaRepository(LamaDbContext context) : IVentaRepository
{
    public async Task AddAsync(Venta venta, CancellationToken cancellationToken = default)
    {
        await context.Ventas.AddAsync(venta, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await context.SaveChangesAsync(cancellationToken);
    }
}
