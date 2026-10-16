using FluentValidation;

namespace Application.Features.Authentication.EmailSignUp;

public sealed class EmailSignUpRequestValidator : AbstractValidator<EmailSignUpRequest>
{
    public EmailSignUpRequestValidator()
    {
        RuleFor(request => request.Email).NotEmpty().EmailAddress();
        RuleFor(request => request.Password).NotEmpty();
    }
}
