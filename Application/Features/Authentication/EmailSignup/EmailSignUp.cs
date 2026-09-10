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
    ILogger<EmailSignUp> logger)
{
    public async Task<EmailSignUpResponse> ExecuteAsync(EmailSignUpRequest request)
    {
        var user = User.Create(request.Email);

        var hashedPassword = passwordHasher.HashPassword(user, request.Password);
        var credential = Credential.Create(user.Id, SignUpProvider.Email, hashedPassword);

        user.AddCredential(credential);

        await userRepository.AddAsync(user);

        var accessToken = tokenService.GenerateAccessToken(user);
        var refreshToken = tokenService.GenerateRefreshToken(user);

        // persist the refresh token.

        return new EmailSignUpResponse(accessToken, refreshToken);
    }
}