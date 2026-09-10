using Domain.Entities;

namespace Application.Abstractions.Authentication;

public interface ITokenService
{
    public string GenerateAccessToken(User user);
    public string GenerateRefreshToken(User user);
}