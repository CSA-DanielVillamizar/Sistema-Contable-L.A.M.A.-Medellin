using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LAMAMedellin.Infrastructure.Persistence.Repositories;

public sealed class BancoRepository(LamaDbContext dbContext) : IBancoRepository
{
    public Task<List<Banco>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.Bancos
            .OrderBy(banco => banco.NumeroCuenta)
            .ToListAsync(cancellationToken);
    }

    public Task<Banco?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return dbContext.Bancos.FirstOrDefaultAsync(banco => banco.Id == id, cancellationToken);
    }

    public async Task<Banco?> GetDefaultAsync(CancellationToken cancellationToken = default)
    {
        var bancoPrincipal = await dbContext.Bancos
            .FirstOrDefaultAsync(banco => banco.NumeroCuenta == "Bancolombia Ahorros", cancellationToken);

        if (bancoPrincipal is not null)
        {
            return bancoPrincipal;
        }

        return await dbContext.Bancos
            .OrderBy(banco => banco.NumeroCuenta)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
