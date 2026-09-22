using System.Reflection;
using Application.Features.Authentication.EmailSignUp;
using Application.Features.Authentication.RefreshAccessToken;
using Application.Features.Authentication.SignInWithEmail;
using Application.Features.Workout.ExerciseModule.CreateExercise;
using Application.Features.Workout.ExerciseModule.GetExercise;
using Application.Features.Workout.WorkoutTemplateModule.CreateWorkoutTemplate;
using Domain.Entities;
using FluentValidation;
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
        services.AddScoped<GetExerciseHandler>();
        services.AddScoped<CreateWorkoutTemplateHandler>();
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }
}