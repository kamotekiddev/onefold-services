using Application.Abstractions.Authentication;
using Application.Abstractions.Persistence;
using Application.Dtos;
using Microsoft.Extensions.Logging;

namespace Application.Features.Workout.Session.GetSessionHistory;

public sealed class GetSessionsHandler(
    IWorkoutSessionRepository workoutSessionRepository,
    ICurrentUser currentUser,
    ILogger<GetSessionsHandler> logger)
{
    public async Task<IReadOnlyCollection<WorkoutSessionListItemDto>> ExecuteAsync()
    {
        var userId = currentUser.UserId;

        logger.LogInformation(
            "Getting workout sessions for user {UserId}.",
            userId);

        var sessions = await workoutSessionRepository.GetAllByUserIdAsync(userId);

        return sessions
            .Select(x => new WorkoutSessionListItemDto(
                x.Id,
                x.WorkoutTemplateId,
                x.WorkoutTemplate.Name,
                x.StartedAt,
                x.CompletedAt))
            .ToList();
    }
}