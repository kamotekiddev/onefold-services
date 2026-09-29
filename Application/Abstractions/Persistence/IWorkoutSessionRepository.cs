using Domain.Entities.Workout.Session;

namespace Application.Abstractions.Persistence;

public interface IWorkoutSessionRepository
{
    void Add(WorkoutSession session);

    Task<WorkoutSession?> GetByIdAsync(Guid sessionId);
}