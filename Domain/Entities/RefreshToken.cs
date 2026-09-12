using Domain.Entities.Exceptions;

namespace Domain.Entities;

public class RefreshToken : Entity
{
    public required Guid UserId { get; init; }
    public required string Value { get; init; }
    public required DateTimeOffset ExpiresAt { get; init; }
    public DateTimeOffset? RevokedAt { get; private set; }

    public User User { get; private set; }

    private RefreshToken()
    {
    }

    public static RefreshToken Create(Guid userId, string value, DateTime expiresAt)
    {
        if (expiresAt < DateTime.Now) throw new InvalidTokenExpirationException();
        if (userId == Guid.Empty) throw new InvalidIdException();
        if (string.IsNullOrWhiteSpace(value)) throw new InvalidTokenException();

        return new RefreshToken()
        {
            UserId = userId,
            Value = value,
            ExpiresAt = expiresAt
        };
    }

    public void Revoke()
    {
        RevokedAt = DateTimeOffset.UtcNow;
    }
}