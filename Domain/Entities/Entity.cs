namespace Domain.Entities;

public abstract class Entity
{
    public Guid Id { get; init; }
    public DateTime CreatedAt { get; set; }
}