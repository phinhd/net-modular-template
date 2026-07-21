namespace BuildingBlocks.Domain.Common;

/// <summary>
/// Base class for every entity in every module.
/// Keep this free of any persistence or framework concerns.
/// </summary>
public abstract class BaseEntity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();

    public DateTime CreatedAtUtc { get; protected set; } = DateTime.UtcNow;

    public DateTime? UpdatedAtUtc { get; protected set; }

    public bool IsDeleted { get; protected set; }

    protected void MarkUpdated() => UpdatedAtUtc = DateTime.UtcNow;

    public void SoftDelete()
    {
        IsDeleted = true;
        MarkUpdated();
    }
}

/// <summary>
/// Marker interface: implement on the root entity of an aggregate
/// (e.g. the entity whose Repository is exposed to the Application layer).
/// </summary>
public interface IAggregateRoot
{
}
