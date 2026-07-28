using LAMAMedellin.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LAMAMedellin.Infrastructure.Persistence.Configurations;

public sealed class PeriodoContableConfiguration : IEntityTypeConfiguration<PeriodoContable>
{
    public void Configure(EntityTypeBuilder<PeriodoContable> builder)
    {
        builder.ToTable("PeriodosContables");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Anio).IsRequired();
        builder.Property(p => p.Mes).IsRequired();

        builder.Property(p => p.Estado)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(p => p.ValidadoPor).HasMaxLength(256);
        builder.Property(p => p.CerradoPor).HasMaxLength(256);

        // Un unico periodo por anio y mes: dos filas para el mismo mes
        // permitirian cerrar una y seguir operando contra la otra.
        builder.HasIndex(p => new { p.Anio, p.Mes })
            .IsUnique()
            .HasDatabaseName("IX_PeriodosContables_AnioMes");

        builder.HasQueryFilter(p => !p.IsDeleted);
    }
}
