using LAMAMedellin.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LAMAMedellin.Infrastructure.Persistence.Configurations;

public sealed class MapeoContableConfiguration : IEntityTypeConfiguration<MapeoContable>
{
    public void Configure(EntityTypeBuilder<MapeoContable> builder)
    {
        builder.ToTable("MapeosContables");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.TipoOperacion)
            .IsRequired();

        builder.HasOne(m => m.CuentaContable)
            .WithMany()
            .HasForeignKey(m => m.CuentaContableId)
            .OnDelete(DeleteBehavior.Restrict);

        // Una operacion tiene exactamente una cuenta. Dos filas para la misma
        // operacion dejarian sin definir cual manda.
        builder.HasIndex(m => m.TipoOperacion)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        builder.HasQueryFilter(m => !m.IsDeleted);
    }
}
