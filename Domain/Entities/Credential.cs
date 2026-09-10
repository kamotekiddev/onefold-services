namespace Domain.Entities;

public enum SignUpProvider
{
    Email,
    Google,
    Apple
}

public class Credential : Entity
{
    public Guid UserId { get; init; }
    public SignUpProvider Provider { get; init; }
    public string Value { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public User User { get; private set; }

    public static Credential Create(Guid userId, SignUpProvider provider, string value)
    {
        return new Credential
        {
            UserId = userId,
            Provider = provider,
            Value = value,
        };
    }
}