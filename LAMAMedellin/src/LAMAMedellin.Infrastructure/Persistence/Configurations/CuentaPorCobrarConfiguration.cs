using LAMAMedellin.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LAMAMedellin.Infrastructure.Persistence.Configurations;

public sealed class CuentaPorCobrarConfiguration : IEntityTypeConfiguration<CuentaPorCobrar>
{
    public void Configure(EntityTypeBuilder<CuentaPorCobrar> builder)
    {
        builder.ToTable("CuentasPorCobrar");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.MiembroId)
            .IsRequired();

        builder.Property(c => c.Periodo)
            .HasMaxLength(7)
            .IsRequired();

        builder.Property(c => c.ValorEsperadoCOP)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(c => c.SaldoPendienteCOP)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(c => c.Estado)
            .HasConversion<int>()
            .IsRequired();

        builder.HasOne(c => c.Miembro)
            .WithMany()
            .HasForeignKey(c => c.MiembroId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.HasIndex(c => new { c.MiembroId, c.Periodo })
            .IsUnique();

        builder.HasQueryFilter(c => !c.IsDeleted);
    }
}
