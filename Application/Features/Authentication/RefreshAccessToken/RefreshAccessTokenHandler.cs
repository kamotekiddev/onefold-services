using Application.Abstractions.Authentication;
using Application.Abstractions.Persistence;
using Microsoft.Extensions.Logging;

namespace Application.Features.Authentication.RefreshAccessToken;

public sealed class RefreshAccessTokenHandler(
    IRefreshTokenRepository refreshTokenRepository,
    ITokenService tokenService,
    IUnitOfWork unitOfWork,
    ILogger<RefreshAccessTokenHandler> logger)
{
    public async Task<RefreshAccessTokenResponse> ExecuteAsync(RefreshAccessTokenRequest request)
    {
        var refreshToken = await refreshTokenRepository.GetByValueWithUser(request.RefreshToken);

        if (refreshToken is null)
        {
            logger.LogWarning("Token refresh failed. Refresh does not exist.");
            throw new InvalidOperationException("Invalid refresh token.");
        }

        if (refreshToken.ExpiresAt <= DateTimeOffset.UtcNow || refreshToken.RevokedAt != null)
        {
            logger.LogWarning("Token refresh failed. either revoked or expired");
            throw new InvalidOperationException("Invalid refresh token.");
        }

        var user = refreshToken.User;
        refreshToken.Revoke();

        var accessToken = tokenService.GenerateAccessToken(user);
        var newRefreshToken = tokenService.GenerateRefreshToken(user);

        refreshTokenRepository.Add(newRefreshToken);
        await unitOfWork.SaveChangesAsync();

        logger.LogInformation(
            "Token refresh successful. UserId:{UserId}",
            user.Id);

        return new RefreshAccessTokenResponse(accessToken, newRefreshToken.Value);
    }
}