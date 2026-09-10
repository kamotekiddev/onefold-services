using Application.Abstractions.Persistence;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Application.Features.Authentication.EmailSignUp;

public sealed class EmailSignUp(IUserRepository userRepository, ILogger<EmailSignUp> logger)
{
    public async Task<EmailSignUpResponse> ExecuteAsync(EmailSignUpRequest request)
    {
        var user = User.Create(request.Email);

        var credential = Credential.Create(user.Id, SignUpProvider.Email, request.Password);

        user.AddCredential(credential);

        await userRepository.AddAsync(user);

        // generate auth tokens;
        throw new NotImplementedException();
    }
}