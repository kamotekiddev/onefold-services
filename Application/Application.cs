using System.Reflection;
using Application.Features.Authentication.EmailSignUp;
using Application.Features.Authentication.RefreshAccessToken;
using Application.Features.Authentication.SignInWithEmail;
using Application.Features.Workout.Exercise.CreateExercise;
using Application.Features.Workout.ExerciseModule.GetExercise;
using Application.Features.Workout.Session.GetSessionById;
using Application.Features.Workout.Session.GetSessionHistory;
using Application.Features.Workout.Session.SaveSession;
using Application.Features.Workout.Template.CreateWorkoutTemplate;
using Application.Features.Workout.Template.GetWorkoutTemplateById;
using Application.Features.Workout.Template.UpdateWorkoutTemplate;
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
        services.AddScoped<UpdateWorkoutTemplateHandler>();
        services.AddScoped<SaveSessionHandler>();
        services.AddScoped<GetSessionByIdHandler>();
        services.AddScoped<GetSessionsHandler>();
        services.AddScoped<GetWorkoutTemplateByIdHandler>();

        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }
}