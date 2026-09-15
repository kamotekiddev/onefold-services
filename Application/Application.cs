using Application.Features.Authentication.EmailSignUp;
using Application.Features.Authentication.RefreshAccessToken;
using Application.Features.Authentication.SignInWithEmail;
using Application.Features.Workout.CreateExercise;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class Application
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<EmailSignUp>();
        services.AddScoped<SignInWithEmailHandler>();
        services.AddScoped<RefreshAccessTokenHandler>();
        services.AddScoped<CreateExerciseHandler>();
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

        return services;
    }
}