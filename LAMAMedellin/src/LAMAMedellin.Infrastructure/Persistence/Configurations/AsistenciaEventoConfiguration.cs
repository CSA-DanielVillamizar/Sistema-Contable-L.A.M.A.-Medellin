using LAMAMedellin.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LAMAMedellin.Infrastructure.Persistence.Configurations;

public sealed class AsistenciaEventoConfiguration : IEntityTypeConfiguration<AsistenciaEvento>
{
    public void Configure(EntityTypeBuilder<AsistenciaEvento> builder)
    {
        builder.ToTable("AsistenciasEvento");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.EventoId)
            .IsRequired();

        builder.Property(a => a.MiembroId)
            .IsRequired();

        builder.Property(a => a.Asistio)
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(a => a.Observaciones)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(a => a.CuotaAplicadaCOP)
            .HasColumnType("decimal(18,2)")
            .IsRequired(false);

        builder.HasIndex(a => new { a.EventoId, a.MiembroId })
            .IsUnique();

        builder.HasOne(a => a.Evento)
            .WithMany(e => e.Asistencias)
            .HasForeignKey(a => a.EventoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Miembro)
            .WithMany()
            .HasForeignKey(a => a.MiembroId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(a => !a.IsDeleted);
    }
}
