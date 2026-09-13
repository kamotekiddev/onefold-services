using System.Net.Mail;
using Domain.Entities.Exceptions;
using Domain.Entities.Workout;

namespace Domain.Entities;

public class User : Entity
{
    public string Email { get; init; } = string.Empty;
    public bool IsEmailVerified { get; private set; }

    public ICollection<Credential> Credentials { get; private set; } = [];
    public ICollection<RefreshToken> RefreshTokens { get; private set; } = [];
    public Profile? Profile { get; private set; }
    public ICollection<Exercise> Exercises { get; private set; } = [];

    private User()
    {
    }

    public static User Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email) || !MailAddress.TryCreate(email, out _))
            throw new InvalidEmailException();

        return new User
        {
            Email = email,
        };
    }

    public void AddCredential(Credential credential)
    {
        if (Credentials.Any(c => c.Provider == credential.Provider))
            throw new CredentialAlreadyExistException();

        Credentials.Add(credential);
    }

    public void UpdateProfile(Profile profile)
    {
        Profile = profile;
    }
}