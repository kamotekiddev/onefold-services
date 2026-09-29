using Application.Abstractions.Authentication;
using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using Application.Dtos;
using Microsoft.Extensions.Logging;

namespace Application.Features.Workout.Session.GetSessionById;

public sealed class GetSessionByIdHandler(
    IWorkoutSessionRepository workoutSessionRepository,
    ICurrentUser currentUser,
    ILogger<GetSessionByIdHandler> logger)
{
    public async Task<WorkoutSessionDto> ExecuteAsync(
        Guid sessionId
    )
    {
        var userId = currentUser.UserId;

        logger.LogInformation(
            "Getting workout session {SessionId} for user {UserId}.",
            sessionId,
            userId);

        var session = await workoutSessionRepository.GetByIdAsync(
            sessionId);

        if (session is null)
            throw new NotFoundException("Workout session not found.");

        if (session.UserId != userId)
            throw new ForbiddenException("You do not have access to this workout session.");

        return new WorkoutSessionDto(
            session.Id,
            session.WorkoutTemplateId,
            session.StartedAt,
            session.CompletedAt,
            session.Exercises
                .OrderBy(x => x.SortIndex)
                .Select(x => new WorkoutSessionExerciseDto(
                    x.Id,
                    x.ExerciseId,
                    x.Exercise.Name,
                    x.TargetSet,
                    x.TargetReps,
                    x.RestInSeconds,
                    x.SortIndex,
                    x.Sets
                        .OrderBy(s => s.SetNumber)
                        .Select(s => new WorkoutSetDto(
                            s.Id,
                            s.SetNumber,
                            s.Reps,
                            s.Weight))
                        .ToList()))
                .ToList());
    }
}