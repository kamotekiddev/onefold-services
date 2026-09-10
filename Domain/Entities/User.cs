using Domain.Entities.Exceptions;

namespace Domain.Entities;

public class User : Entity
{
    public string Email { get; init; } = string.Empty;
    public bool IsEmailVerified { get; private set; }

    public ICollection<Credential> Credentials { get; private set; } = [];
    public Profile? Profile { get; private set; }

    public static User Create(string email)
    {
        return new User
        {
            Email = email,
        };
    }

    public void AddCredential(Credential credential)
    {
        if (Credentials.Contains(credential)) throw new CredentialAlreadyExistException();
        Credentials.Add(credential);
    }

    public void UpdateProfile(Profile profile)
    {
        Profile = profile;
    }
}