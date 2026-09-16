using Application.Abstractions.Authentication;
using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using Application.Dtos;
using Microsoft.Extensions.Logging;

namespace Application.Features.Workout.GetExercise;

public sealed class GetExerciseHandler(
    IExerciseRepository exerciseRepository,
    ICurrentUser currentUser,
    ILogger<GetExerciseHandler> logger)
{
    public async Task<ExerciseDto?> ExecuteAsync(Guid id)
    {
        var exercise = await exerciseRepository.GetById(id);

        if (exercise is null)
        {
            logger.LogWarning("ExerciseId:{Id} does not exist.", id);
            throw new NotFoundException("Exercise with the given Id does not exist.");
        }

        if (exercise.UserId != currentUser.UserId)
        {
            logger.LogWarning("ExerciseId:{id} does not belong to UserId:{userId}", id,
                currentUser.UserId);
            throw new ForbiddenException("This exercise does not belong to this user.");
        }

        return new ExerciseDto(exercise.Id, exercise.Name);
    }
}