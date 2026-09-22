using Application.Abstractions.Authentication;
using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using Domain.Entities.Workout;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Application.Features.Workout.WorkoutTemplateModule.CreateWorkoutTemplate;

public sealed class CreateWorkoutTemplateHandler(
    IWorkoutTemplateRepository workoutTemplateRepository,
    IExerciseRepository exerciseRepository,
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork,
    IValidator<CreateWorkoutTemplateRequest> validator,
    ILogger<CreateWorkoutTemplateHandler> logger)
{
    public async Task<CreateWorkoutTemplateResponse> ExecuteAsync(CreateWorkoutTemplateRequest request)
    {
        await validator.ValidateAndThrowAsync(request);

        var userId = currentUser.UserId;

        var templateName = request.Name.Trim();
        var description = request.Description?.Trim();

        var alreadyExists = await workoutTemplateRepository.CheckUserOwnedByNameAsync(userId, templateName);
        if (alreadyExists)
        {
            logger.LogWarning("Workout template: {templateName} already exist.", templateName);
            throw new ConflictException("Workout template already exist.");
        }

        var exerciseIds = ExtractExerciseIdsFromRequest(request.Exercises);
        var availableIds = GetAvailableIds(await exerciseRepository.GetAvailableByIdsAsync(userId, exerciseIds));
        var invalidIds = GetInvalidIds(exerciseIds, availableIds);

        if (invalidIds.Count > 0)
        {
            logger.LogWarning(
                "One or more exercises are unavailable. UserId: {UserId}, Ids:{ids}",
                userId, string.Join(",", invalidIds));

            throw new NotFoundException(
                "One or more exercises were not found.");
        }

        var workoutTemplate = WorkoutTemplate.Create(
            userId,
            templateName,
            request.RestInMinutes,
            description);

        foreach (var exercise in request.Exercises)
        {
            var workoutExercise = WorkoutTemplateExercise.Create(
                workoutTemplate.Id,
                exercise.ExerciseId,
                exercise.TargetReps,
                exercise.RestPerSetInSeconds,
                exercise.OrderIdx);

            workoutTemplate.AddExercise(workoutExercise);
        }

        workoutTemplateRepository.Add(workoutTemplate);
        await unitOfWork.SaveChangesAsync();

        logger.LogInformation(
            "Successfully created WorkoutTemplate: {WorkoutTemplateId} with {ExerciseCount} exercises.",
            workoutTemplate.Id,
            workoutTemplate.WorkoutExercises.Count);

        return new CreateWorkoutTemplateResponse(workoutTemplate.Id);
    }

    private HashSet<Guid> GetInvalidIds(
        IReadOnlyCollection<Guid> exerciseIds,
        IReadOnlyCollection<Guid> availableExerciseIds)
    {
        return exerciseIds.Where(id => !availableExerciseIds.Contains(id)).ToHashSet();
    }

    private HashSet<Guid> GetAvailableIds(IReadOnlyCollection<Exercise> exercises)
    {
        return exercises.Select(e => e.Id).ToHashSet();
    }

    private HashSet<Guid> ExtractExerciseIdsFromRequest(IReadOnlyCollection<WorkoutItem> exercises)
    {
        return exercises.Select(e => e.ExerciseId).ToHashSet();
    }
}