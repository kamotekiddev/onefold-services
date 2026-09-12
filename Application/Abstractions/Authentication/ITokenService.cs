using Domain.Entities;

namespace Application.Abstractions.Authentication;

public interface ITokenService
{
    public string GenerateAccessToken(User user);
    public RefreshToken GenerateRefreshToken(User user);
}