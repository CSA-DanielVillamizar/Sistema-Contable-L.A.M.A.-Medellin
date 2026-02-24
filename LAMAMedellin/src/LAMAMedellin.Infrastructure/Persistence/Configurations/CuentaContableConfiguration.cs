using LAMAMedellin.Domain.Entities;
using LAMAMedellin.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LAMAMedellin.Infrastructure.Persistence.Configurations;

public sealed class CuentaContableConfiguration : IEntityTypeConfiguration<CuentaContable>
{
    public void Configure(EntityTypeBuilder<CuentaContable> builder)
    {
        builder.ToTable("CuentasContables");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Codigo)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(c => c.Descripcion)
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(c => c.Naturaleza)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(c => c.PermiteMovimiento)
            .IsRequired();

        builder.Property(c => c.ExigeTercero)
            .IsRequired();

        builder.Property(c => c.CuentaPadreId)
            .IsRequired(false);

        // El Nivel se calcula desde el Codigo; no se persiste en la base de datos.
        builder.Ignore(c => c.Nivel);

        builder.HasIndex(c => c.Codigo)
            .IsUnique();

        // Relación jerárquica auto-referencial (padre → hijos)
        builder.HasOne(c => c.CuentaPadre)
            .WithMany()
            .HasForeignKey(c => c.CuentaPadreId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        // Excluir registros con soft delete del filtro global
        builder.HasQueryFilter(c => !c.IsDeleted);
    }
}
