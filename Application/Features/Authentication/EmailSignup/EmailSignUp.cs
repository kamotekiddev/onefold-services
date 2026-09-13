using Application.Abstractions.Authentication;
using Application.Abstractions.Persistence;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Application.Features.Authentication.EmailSignUp;

public sealed class EmailSignUp(
    IUserRepository userRepository,
    ITokenService tokenService,
    IPasswordHasher<User> passwordHasher,
    IRefreshTokenRepository refreshTokenRepository,
    IUnitOfWork unitOfWork,
    ILogger<EmailSignUp> logger)
{
    public async Task<EmailSignUpResponse> ExecuteAsync(EmailSignUpRequest request)
    {
        var existingUser = await userRepository.GetByEmailAsync(request.Email);

        if (existingUser is not null)
            throw new InvalidOperationException("Email is already taken.");

        var user = User.Create(request.Email);

        var hashedPassword = passwordHasher.HashPassword(user, request.Password);
        var credential = Credential.Create(user.Id, CredentialProvider.Email, hashedPassword);

        user.AddCredential(credential);

        userRepository.Add(user);

        var accessToken = tokenService.GenerateAccessToken(user);
        var refreshToken = tokenService.GenerateRefreshToken(user);

        refreshTokenRepository.Add(refreshToken);

        try
        {
            await unitOfWork.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Sign up failed while saving to database. ProviderId:{ProviderId} UserId:{UserId}",
                nameof(CredentialProvider.Email), user.Id);

            throw;
        }

        logger.LogInformation("Sign up successful. Provider:{Provider}, UserId:{UserId}", nameof(CredentialProvider.Email),
            user.Id);

        return new EmailSignUpResponse(accessToken, refreshToken.Value);
    }
}