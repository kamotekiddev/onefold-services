namespace Application.Features.Authentication.EmailSignUp;

public record EmailSignUpResponse(string AccessToken, string RefreshToken);