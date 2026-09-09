namespace Domain.Entities;

public enum Provider
{
    Password,
    Google,
    Apple
}

public class Credential : Entity
{
    public Guid UserId { get; init; }
    public Provider Provider { get; init; }
    public string Value { get; private set; }

    public DateTime PasswordChangedAt { get; private set; }
    public User User { get; private set; }
}