using Application.Abstractions.Authentication;
using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using Domain.Entities.Workout.Session;
using Microsoft.Extensions.Logging;

namespace Application.Features.Workout.Session.SaveSession;

public class SaveSessionHandler(
    ICurrentUser currentUser,
    IWorkoutTemplateRepository workoutTemplateRepository,
    IWorkoutSessionRepository workoutSessionRepository,
    IUnitOfWork unitOfWork,
    ILogger<SaveSessionHandler> logger)
{
    public async Task<SaveSessionResponse> ExecuteAsync(SaveSessionRequest request)
    {
        var userId = currentUser.UserId;

        var template = await workoutTemplateRepository.GetByIdWithExercisesAsync(request.WorkoutTemplateId);

        if (template is null)
        {
            logger.LogWarning("Invalid TemplateId:{templateId}", request.WorkoutTemplateId);
            throw new NotFoundException("Invalid TemplateId.");
        }

        if (template.UserId != userId)
        {
            logger.LogWarning(
                "UserId:{userId} attempted to load TemplateId:{templateId} that belongs to UserId:{ownerId}",
                userId,
                template.Id,
                template.UserId);

            throw new ForbiddenException("Template does not belong to user.");
        }

        var session = WorkoutSession.Create(template.Id, userId, request.StartedAt, request.CompletedAt);
        var templateExercises = template.WorkoutExercises.ToDictionary(x => x.ExerciseId);

        foreach (var exerciseRequest in request.Exercises)
        {
            var templateExercise = templateExercises[exerciseRequest.ExerciseId];
            var sessionExercise = WorkoutSessionExercise.Create(
                session.Id,
                exerciseRequest.ExerciseId,
                templateExercise.TargetSet,
                templateExercise.TargetReps,
                templateExercise.RestInSeconds,
                templateExercise.SortIndex);

            foreach (var set in exerciseRequest.Sets)
            {
                sessionExercise.AddSet(set.SetNumber, set.Reps, set.Weight);
            }

            session.AddExercise(sessionExercise);
        }

        workoutSessionRepository.Add(session);

        await unitOfWork.SaveChangesAsync();

        logger.LogInformation("Successfully saved the SessionId:{sessionId} for UserId:{userId}", session.Id, userId);

        return new SaveSessionResponse(session.Id);
    }
}