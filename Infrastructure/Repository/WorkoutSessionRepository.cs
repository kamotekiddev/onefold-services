using Application.Abstractions.Persistence;
using Domain.Entities.Workout.Session;
using Infrastructure.Database;

namespace Infrastructure.Repository;

public class WorkoutSessionRepository(AppDbContext db) : IWorkoutSessionRepository
{
    public void Add(WorkoutSession session)
    {
        db.WorkoutSessions.Add(session);
    }
}