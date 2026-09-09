namespace Domain.Entities;

public class User : Entity
{
    public string? Email { get; init; }
    public bool IsEmailVerified { get; private set; }

    public ICollection<Credential> Credentials { get; private set; }
    public Profile? Profile { get; private set; }
}