namespace LAMAMedellin.Domain.Common;

public abstract class BaseEntity
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public bool IsDeleted { get; private set; } = false;

    public void MarcarComoEliminado()
    {
        IsDeleted = true;
    }
}
