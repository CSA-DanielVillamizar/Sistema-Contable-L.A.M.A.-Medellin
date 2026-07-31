using LAMAMedellin.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LAMAMedellin.Infrastructure.Persistence.Configurations;

public sealed class ActividadProyectoConfiguration : IEntityTypeConfiguration<ActividadProyecto>
{
    public void Configure(EntityTypeBuilder<ActividadProyecto> builder)
    {
        builder.ToTable("ActividadesProyecto");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Nombre)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(a => a.Descripcion)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(a => a.Responsable)
            .HasMaxLength(200);

        builder.Property(a => a.FechaInicioPlanificada)
            .HasColumnType("date")
            .IsRequired();

        builder.Property(a => a.FechaFinPlanificada)
            .HasColumnType("date")
            .IsRequired();

        builder.Property(a => a.PresupuestoAsignado)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(a => a.Estado)
            .IsRequired();

        builder.HasOne(a => a.ProyectoSocial)
            .WithMany()
            .HasForeignKey(a => a.ProyectoSocialId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(a => a.ProyectoSocialId);

        builder.HasQueryFilter(a => !a.IsDeleted);
    }
}
