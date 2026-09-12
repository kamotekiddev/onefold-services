namespace Domain.Entities;

public abstract class Entity
{
    public Guid Id { get; protected init; } = Guid.NewGuid();
    public DateTimeOffset CreatedAt { get; protected init; } = DateTime.UtcNow;
    public DateTimeOffset? UpdatedAt { get; private set; }
}