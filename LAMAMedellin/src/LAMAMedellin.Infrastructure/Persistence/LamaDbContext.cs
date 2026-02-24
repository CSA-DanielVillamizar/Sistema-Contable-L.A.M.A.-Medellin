using LAMAMedellin.Domain.Common;
using LAMAMedellin.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LAMAMedellin.Infrastructure.Persistence;

public sealed class LamaDbContext(DbContextOptions<LamaDbContext> options) : DbContext(options)
{
    public DbSet<Banco> Bancos => Set<Banco>();
    public DbSet<CentroCosto> CentrosCosto => Set<CentroCosto>();
    public DbSet<CuentaPorCobrar> CuentasPorCobrar => Set<CuentaPorCobrar>();
    public DbSet<CuotaAsamblea> CuotasAsamblea => Set<CuotaAsamblea>();
    public DbSet<Miembro> Miembros => Set<Miembro>();
    public DbSet<Transaccion> Transacciones => Set<Transaccion>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LamaDbContext).Assembly);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        AplicarSoftDelete();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void AplicarSoftDelete()
    {
        var entradasEliminadas = ChangeTracker
            .Entries<BaseEntity>()
            .Where(entry => entry.State == EntityState.Deleted)
            .ToList();

        foreach (var entrada in entradasEliminadas)
        {
            entrada.State = EntityState.Modified;
            entrada.Entity.MarcarComoEliminado();
        }
    }
}
