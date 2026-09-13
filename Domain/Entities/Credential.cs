namespace Domain.Entities;

public enum CredentialProvider
{
    Email,
    Google,
    Apple
}

public class Credential : Entity
{
    public Guid UserId { get; init; }
    public CredentialProvider Provider { get; init; }
    public string Value { get; private set; }

    public User User { get; private set; }

    private Credential()
    {
    }

    public static Credential Create(Guid userId, CredentialProvider provider, string value)
    {
        return new Credential
        {
            UserId = userId,
            Provider = provider,
            Value = value,
        };
    }
}