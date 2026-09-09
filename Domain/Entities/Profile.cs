namespace Domain.Entities;

public class Profile : Entity
{
    public Guid UserId { get; init; }
    public string DisplayName { get; private set; }

    public User User { get; set; }
}