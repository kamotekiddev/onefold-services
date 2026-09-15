using Application.Abstractions.Authentication;
using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using Domain.Entities.Workout;
using Microsoft.Extensions.Logging;

namespace Application.Features.Workout.CreateExercise;

public sealed class CreateExerciseHandler(
    IExerciseRepository exerciseRepository,
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser,
    ILogger<CreateExerciseHandler> logger)
{
    public async Task<CreateExerciseResponse> ExecuteAsync(CreateExerciseRequest request)
    {
        var name = request.Name.Trim();
        var exercise = await exerciseRepository.GetByNameAsync(name);

        if (exercise is not null)
        {
            logger.LogWarning("Attempted to create exercise {exercise} that already exist.", name);
            throw new ConflictException("Exercise already exist.");
        }

        exercise = Exercise.Create(request.Name);
        exercise.AttachToUser(currentUser.UserId);

        exerciseRepository.Add(exercise);
        await unitOfWork.SaveChangesAsync();

        logger.LogInformation("Successfully created exercise. ExerciseId:{ExerciseId}", exercise.Id);

        return new CreateExerciseResponse(exercise.Id);
    }
}