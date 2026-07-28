using LAMAMedellin.Application.Common.Interfaces.Services;
using LAMAMedellin.Domain.Common;
using LAMAMedellin.Domain.Entities;
using LAMAMedellin.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace LAMAMedellin.Infrastructure.Persistence;

public sealed class LamaDbContext(
    DbContextOptions<LamaDbContext> options,
    IUsuarioActual? usuarioActual = null) : DbContext(options)
{
    public DbSet<Caja> Cajas => Set<Caja>();
    public DbSet<Ingreso> Ingresos => Set<Ingreso>();
    public DbSet<Egreso> Egresos => Set<Egreso>();
    public DbSet<Banco> Bancos => Set<Banco>();
    public DbSet<CentroCosto> CentrosCosto => Set<CentroCosto>();
    public DbSet<CuentaContable> CuentasContables => Set<CuentaContable>();
    public DbSet<Comprobante> Comprobantes => Set<Comprobante>();
    public DbSet<AsientoContable> AsientosContables => Set<AsientoContable>();
    public DbSet<ConceptoCobro> ConceptosCobro => Set<ConceptoCobro>();
    public DbSet<CuentaPorCobrar> CuentasPorCobrar => Set<CuentaPorCobrar>();
    public DbSet<CuotaAsamblea> CuotasAsamblea => Set<CuotaAsamblea>();
    public DbSet<TarifaCuota> TarifasCuota => Set<TarifaCuota>();
    public DbSet<Donacion> Donaciones => Set<Donacion>();
    public DbSet<Donante> Donantes => Set<Donante>();
    public DbSet<ProyectoSocial> ProyectosSociales => Set<ProyectoSocial>();
    public DbSet<Beneficiario> Beneficiarios => Set<Beneficiario>();
    public DbSet<Miembro> Miembros => Set<Miembro>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Evento> Eventos => Set<Evento>();
    public DbSet<AsistenciaEvento> AsistenciasEvento => Set<AsistenciaEvento>();
    public DbSet<Transaccion> Transacciones => Set<Transaccion>();
    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<MovimientoInventario> MovimientosInventario => Set<MovimientoInventario>();
    public DbSet<ConsecutivoComprobante> ConsecutivosComprobante => Set<ConsecutivoComprobante>();
    public DbSet<PeriodoContable> PeriodosContables => Set<PeriodoContable>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LamaDbContext).Assembly);

        // Columnas de auditoria: acotadas para no terminar en nvarchar(max).
        foreach (var tipo in modelBuilder.Model.GetEntityTypes()
                     .Where(t => typeof(BaseEntity).IsAssignableFrom(t.ClrType)))
        {
            modelBuilder.Entity(tipo.ClrType)
                .Property(nameof(BaseEntity.CreatedBy))
                .HasMaxLength(256);

            modelBuilder.Entity(tipo.ClrType)
                .Property(nameof(BaseEntity.UpdatedBy))
                .HasMaxLength(256);

            modelBuilder.Entity(tipo.ClrType)
                .Property(nameof(BaseEntity.DeletedBy))
                .HasMaxLength(256);
        }
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        AplicarSoftDelete();
        AnotarAuditoria();
        RechazarMovimientosEnPeriodoCerrado();
        return base.SaveChangesAsync(cancellationToken);
    }

    // La version sincrona tambien debe pasar por aqui: sobrescribir solo la
    // asincrona dejaba una via por la que se salvaban cambios sin soft delete
    // ni auditoria.
    public override int SaveChanges()
    {
        AplicarSoftDelete();
        AnotarAuditoria();
        RechazarMovimientosEnPeriodoCerrado();
        return base.SaveChanges();
    }

    /// <summary>
    /// Impide tocar la contabilidad de un periodo ya cerrado.
    ///
    /// Se valida aqui y no en los manejadores a proposito: todo hecho contable
    /// termina siendo un Comprobante, asi que este es el unico punto por el que
    /// necesariamente pasa. Ponerlo en cada caso de uso dejaria la puerta
    /// abierta a que un caso de uso nuevo se olvide de la regla.
    ///
    /// Los comprobantes de tipo Ajuste si se admiten, porque son justamente el
    /// mecanismo que el backlog define para corregir despues del cierre
    /// (historia 1-5) sin editar el documento de origen.
    /// </summary>
    private void RechazarMovimientosEnPeriodoCerrado()
    {
        var comprobantes = ChangeTracker
            .Entries<Comprobante>()
            .Where(entrada => entrada.State is EntityState.Added or EntityState.Modified)
            .Select(entrada => entrada.Entity)
            .Where(comprobante => comprobante.TipoComprobante != TipoComprobante.Ajuste)
            .ToList();

        if (comprobantes.Count == 0)
        {
            // Sin comprobantes en juego no se consulta nada: el guardian no
            // cuesta en los guardados que no tocan contabilidad.
            return;
        }

        var periodosAfectados = comprobantes
            .Select(comprobante => (comprobante.Fecha.Year, comprobante.Fecha.Month))
            .Distinct()
            .ToList();

        var cerrados = PeriodosContables
            .Where(periodo => periodo.Estado == EstadoPeriodoContable.Cerrado)
            .Select(periodo => new { periodo.Anio, periodo.Mes })
            .ToList();

        var choque = periodosAfectados
            .FirstOrDefault(afectado => cerrados.Any(
                cerrado => cerrado.Anio == afectado.Year && cerrado.Mes == afectado.Month));

        if (choque != default)
        {
            throw new ReglaNegocioException(
                $"El periodo contable {choque.Year}-{choque.Month:D2} esta cerrado. " +
                "Registre un comprobante de ajuste en lugar de modificar el origen.");
        }
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
            MarcarAuditoria(entrada, nameof(BaseEntity.DeletedAt), nameof(BaseEntity.DeletedBy));
        }
    }

    /// <summary>
    /// Sella quien y cuando sobre cada entidad que se va a persistir.
    /// Se escribe por el rastreador de cambios para no exponer setters
    /// publicos en el dominio: ningun manejador puede falsear estos valores.
    /// </summary>
    private void AnotarAuditoria()
    {
        foreach (var entrada in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entrada.State)
            {
                case EntityState.Added:
                    MarcarAuditoria(entrada, nameof(BaseEntity.CreatedAt), nameof(BaseEntity.CreatedBy));
                    break;

                case EntityState.Modified:
                    MarcarAuditoria(entrada, nameof(BaseEntity.UpdatedAt), nameof(BaseEntity.UpdatedBy));
                    break;
            }
        }
    }

    private void MarcarAuditoria(
        Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<BaseEntity> entrada,
        string propiedadFecha,
        string propiedadUsuario)
    {
        entrada.Property(propiedadFecha).CurrentValue = DateTime.UtcNow;
        entrada.Property(propiedadUsuario).CurrentValue = usuarioActual?.Identificador;
    }
}
