using Application.Abstractions.Authentication;
using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using Domain.Entities.Workout;
using Microsoft.Extensions.Logging;

namespace Application.Features.Workout.AddWorkoutExerciseToTemplate;

public class AddWorkoutExerciseToTemplateHandler(
    IWorkoutTemplateRepository workoutTemplateRepository,
    IExerciseRepository exerciseRepository,
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork,
    ILogger<AddWorkoutExerciseToTemplateHandler> logger)
{
    public async Task<AddWorkoutExerciseToTemplateResponse> ExecuteAsync(Guid templateId,
        AddWorkoutExerciseToTemplateRequest request)
    {
        var exerciseId = request.ExerciseId;
        var userId = currentUser.UserId;

        var workoutTemplate = await workoutTemplateRepository.GetByIdAsync(templateId);

        if (workoutTemplate is null)
        {
            logger.LogWarning("Template not found for TemplateId:{id}", templateId);
            throw new NotFoundException("Invalid template id.");
        }

        if (workoutTemplate.UserId != userId)
        {
            logger.LogWarning("Template does not belong to UserId:{userId}", userId);
            throw new ForbiddenException("Invalid template id.");
        }

        var exercise = await exerciseRepository.GetByIdAsync(exerciseId);

        if (exercise is null)
        {
            logger.LogWarning("Exercise not found. ExerciseId:{exerciseId}", exerciseId);
            throw new NotFoundException("Invalid Exercise Id.");
        }

        if (exercise.UserId is not null && exercise.UserId != userId)
        {
            logger.LogWarning(
                "User {UserId} attempted to use Exercise {ExerciseId} owned by {ExerciseOwnerId}",
                userId,
                exerciseId,
                exercise.UserId);

            throw new ForbiddenException("Invalid exercise id.");
        }

        var workoutExercise = WorkoutExercise.Create(
            templateId,
            request.ExerciseId,
            request.TargetReps,
            request.RestPerSetInSeconds,
            request.OrderIdx
        );

        workoutTemplate.AddExercise(workoutExercise);
        await unitOfWork.SaveChangesAsync();

        logger.LogInformation("Successfully added ExerciseId:{exerciseId} to TemplateId:{templateId}",
            request.ExerciseId, templateId);

        return new AddWorkoutExerciseToTemplateResponse(workoutExercise.Id);
    }
}