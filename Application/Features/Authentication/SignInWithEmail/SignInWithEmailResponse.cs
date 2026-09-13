namespace Application.Features.Authentication.SignInWithEmail;

public record SignInWithEmailResponse(string AccessToken, string RefreshToken);