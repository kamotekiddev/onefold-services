using Application.Abstractions.Authentication;
using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using Domain.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Application.Features.Authentication.SignInWithEmail;

public class SignInWithEmailHandler(
    IUserRepository userRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IPasswordHasher<User> passwordHasher,
    ITokenService tokenService,
    IUnitOfWork unitOfWork,
    IValidator<SignInWithEmailRequest> validator,
    ILogger<SignInWithEmailHandler> logger)
{
    public async Task<SignInWithEmailResponse> ExecuteAsync(SignInWithEmailRequest request)
    {
        await validator.ValidateAndThrowAsync(request);

        var user = await userRepository.GetByEmailWithCredentialsAsync(request.Email);

        if (user is null)
            throw new UnAuthorizedException("Invalid email or password.");

        var emailCredential = user.Credentials.SingleOrDefault(c => c.Provider == CredentialProvider.Email);

        if (emailCredential is null)
        {
            logger.LogWarning(
                "Email sign in failed. Email credential not found for UserId:{UserId}",
                user.Id);

            throw new UnAuthorizedException("Invalid email or password.");
        }

        var passwordVerificationResult =
            passwordHasher.VerifyHashedPassword(user, emailCredential.Value, request.Password);

        if (passwordVerificationResult == PasswordVerificationResult.Failed)
        {
            logger.LogWarning(
                "Email sign in failed. the user provided incorrect password for UserId:{UserId}",
                user.Id);

            throw new UnAuthorizedException("Invalid email or password.");
        }

        var accessToken = tokenService.GenerateAccessToken(user);
        var refreshToken = tokenService.GenerateRefreshToken(user);

        refreshTokenRepository.Add(refreshToken);

        await unitOfWork.SaveChangesAsync();

        logger.LogInformation("Login successful for UserId:{UserId}", user.Id);

        return new SignInWithEmailResponse(accessToken, refreshToken.Value);
    }
}
