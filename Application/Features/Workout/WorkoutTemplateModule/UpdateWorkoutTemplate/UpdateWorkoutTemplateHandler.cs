using Application.Abstractions.Authentication;
using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using Domain.Entities.Workout;
using Microsoft.Extensions.Logging;

namespace Application.Features.Workout.WorkoutTemplateModule.UpdateWorkoutTemplate;

public sealed class UpdateWorkoutTemplateHandler(
    IWorkoutTemplateRepository workoutTemplateRepository,
    IExerciseRepository exerciseRepository,
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork,
    ILogger<UpdateWorkoutTemplateHandler> logger)
{
    public async Task<UpdateWorkoutTemplateResponse> ExecuteAsync(Guid templateId, UpdateWorkoutTemplateRequest request)
    {
        var userId = currentUser.UserId;

        var workoutTemplate = await workoutTemplateRepository.GetByIdAsync(templateId);
        if (workoutTemplate is null)
        {
            logger.LogWarning("Invalid TemplateId:{templateId}", templateId);
            throw new NotFoundException("Invalid template id.");
        }

        if (workoutTemplate.UserId != userId)
        {
            logger.LogWarning(
                "UserId:{userId} attempted to access TemplateId:{templateId} which belongs to UserId:{ownerId}",
                userId, templateId,
                workoutTemplate.UserId);

            throw new ForbiddenException("Invalid template id.");
        }

        var requestExerciseIds = ExtractExerciseIdsFromRequest(request);
        var availableIds = GetAvailableIds(await exerciseRepository.GetAvailableByIdsAsync(userId, requestExerciseIds));
        var invalidIds = GetInvalidIds(availableIds, requestExerciseIds);

        if (invalidIds.Count > 0)
        {
            logger.LogWarning("One or more exercise are invalid. InvalidIds:{invalidIds}", invalidIds);
            throw new ConflictException("One or more exercises are invalid.");
        }

        CleanWorkoutTemplateExercises(requestExerciseIds, workoutTemplate);

        foreach (var requestExercise in request.Exercises)
        {
            var workoutExercise =
                workoutTemplate.WorkoutExercises.SingleOrDefault(we => we.ExerciseId == requestExercise.ExerciseId);

            if (workoutExercise is null)
            {
                var newWorkoutExercise = WorkoutExercise.Create(
                    workoutTemplate.Id,
                    requestExercise.ExerciseId,
                    requestExercise.TargetReps,
                    requestExercise.RestPerSetInSeconds,
                    requestExercise.OrderIdx);

                workoutTemplate.AddExercise(newWorkoutExercise);
            }

            else
            {
                workoutExercise.Update(
                    requestExercise.TargetReps,
                    requestExercise.RestPerSetInSeconds,
                    requestExercise.OrderIdx);
            }
        }

        await unitOfWork.SaveChangesAsync();
        logger.LogInformation("Workout Template Updated Successfully");

        return new UpdateWorkoutTemplateResponse(workoutTemplate.Id);
    }

    private ICollection<Guid> ExtractExerciseIdsFromRequest(UpdateWorkoutTemplateRequest request)
    {
        return request.Exercises.Select(e => e.ExerciseId).ToList();
    }

    private ICollection<Guid> GetAvailableIds(IReadOnlyCollection<Exercise> exercises)
    {
        return exercises.Select(e => e.Id).ToList();
    }

    private ICollection<Guid> GetInvalidIds(ICollection<Guid> availableIds, ICollection<Guid> exerciseIds)
    {
        return exerciseIds.Where(id => !availableIds.Contains(id)).ToList();
    }

    private void CleanWorkoutTemplateExercises(ICollection<Guid> requestIds, WorkoutTemplate workoutTemplate)
    {
        foreach (var existing in workoutTemplate.WorkoutExercises)
        {
            var shouldRemove = !requestIds.Contains(existing.ExerciseId);
            if (shouldRemove)
            {
                workoutTemplate.RemoveExercise(existing);
            }
        }
    }
}