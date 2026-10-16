using FluentValidation;

namespace Application.Features.Authentication.SignInWithEmail;

public sealed class SignInWithEmailRequestValidator : AbstractValidator<SignInWithEmailRequest>
{
    public SignInWithEmailRequestValidator()
    {
        RuleFor(request => request.Email).NotEmpty().EmailAddress();
        RuleFor(request => request.Password).NotEmpty();
    }
}
