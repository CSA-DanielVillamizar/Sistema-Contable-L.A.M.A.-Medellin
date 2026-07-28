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

        builder.Property(b => b.Nombre)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(b => b.NumeroCuenta)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(b => b.EsActivo)
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(b => b.SaldoActual)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.HasIndex(b => b.NumeroCuenta)
            .IsUnique();

        // La cuenta contable es la contrapartida de todo movimiento de
        // tesoreria; sin ella la partida doble quedaria incompleta.
        builder.HasOne(b => b.CuentaContable)
            .WithMany()
            .HasForeignKey(b => b.CuentaContableId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.HasQueryFilter(b => !b.IsDeleted);
    }
}
