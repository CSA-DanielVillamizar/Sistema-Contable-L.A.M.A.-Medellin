using LAMAMedellin.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LAMAMedellin.Infrastructure.Persistence.Configurations;

public sealed class BancoConfiguration : IEntityTypeConfiguration<Banco>
{
    public void Configure(EntityTypeBuilder<Banco> builder)
    {
        builder.ToTable("Bancos");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.NumeroCuenta)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(b => b.SaldoActual)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.HasIndex(b => b.NumeroCuenta)
            .IsUnique();

        builder.HasQueryFilter(b => !b.IsDeleted);
    }
}
