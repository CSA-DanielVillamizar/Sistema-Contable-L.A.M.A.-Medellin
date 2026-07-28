namespace LAMAMedellin.Domain.Common;

public abstract class BaseEntity
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public bool IsDeleted { get; private set; } = false;

    // ------------------------------------------------------------------
    // Pista de auditoria
    //
    // Las escribe LamaDbContext al guardar, a traves del rastreador de
    // cambios. No se asignan desde el dominio ni desde los manejadores, para
    // que no exista forma de registrar un movimiento sin dejar rastro.
    //
    // Son nullable a proposito: null significa "anterior a la auditoria", que
    // es la verdad para las filas que ya existian. Ponerles una fecha por
    // defecto habria hecho parecer que todo el historico se creo el dia de la
    // migracion.
    // ------------------------------------------------------------------
    public DateTime? CreatedAt { get; private set; }
    public string? CreatedBy { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public string? UpdatedBy { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public string? DeletedBy { get; private set; }

    public void MarcarComoEliminado()
    {
        IsDeleted = true;
    }
}
