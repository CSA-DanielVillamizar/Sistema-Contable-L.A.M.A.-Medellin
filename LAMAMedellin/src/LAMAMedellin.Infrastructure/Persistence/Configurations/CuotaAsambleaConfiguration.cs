using LAMAMedellin.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LAMAMedellin.Infrastructure.Persistence.Configurations;

public sealed class CuotaAsambleaConfiguration : IEntityTypeConfiguration<CuotaAsamblea>
{
    public void Configure(EntityTypeBuilder<CuotaAsamblea> builder)
    {
        builder.ToTable("CuotasAsamblea");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Anio)
            .IsRequired();

        builder.Property(c => c.ValorMensualCOP)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(c => c.MesInicioCobro)
            .IsRequired();

        builder.Property(c => c.ActaSoporte)
            .HasMaxLength(500);

        builder.HasQueryFilter(c => !c.IsDeleted);
    }
}
