using Application.Abstractions.Authentication;
using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Application.Features.Workout.Exercise.CreateExercise;

public sealed class CreateExerciseHandler(
    IExerciseRepository exerciseRepository,
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser,
    IValidator<CreateExerciseRequest> validator,
    ILogger<CreateExerciseHandler> logger)
{
    public async Task<CreateExerciseResponse> ExecuteAsync(CreateExerciseRequest request)
    {
        await validator.ValidateAndThrowAsync(request);

        var name = request.Name.Trim();
        var exercise = await exerciseRepository.GetByNameAsync(name);

        if (exercise is not null && exercise?.UserId == currentUser.UserId)
        {
            logger.LogWarning("Attempted to create exercise {exercise} that already exist.", name);
            throw new ConflictException("Exercise already exist.");
        }

        exercise = Domain.Entities.Workout.Exercise.Create(request.Name, request.Type, request.Description);
        exercise.AttachToUser(currentUser.UserId);

        exerciseRepository.Add(exercise);
        await unitOfWork.SaveChangesAsync();

        logger.LogInformation("Successfully created exercise. ExerciseId:{ExerciseId}", exercise.Id);

        return new CreateExerciseResponse(exercise.Id);
    }
}