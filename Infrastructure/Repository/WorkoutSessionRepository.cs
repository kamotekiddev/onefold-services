using Application.Abstractions.Persistence;
using Domain.Entities.Workout.Session;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository;

public class WorkoutSessionRepository(AppDbContext db) : IWorkoutSessionRepository
{
    public void Add(WorkoutSession session)
    {
        db.WorkoutSessions.Add(session);
    }

    public async Task<WorkoutSession?> GetByIdAsync(Guid sessionId)
    {
        return await db.WorkoutSessions
            .Include(x => x.Exercises)
            .ThenInclude(x => x.Exercise)
            .Include(x => x.Exercises)
            .ThenInclude(x => x.Sets)
            .FirstOrDefaultAsync(x => x.Id == sessionId);
    }

    public async Task<IReadOnlyCollection<WorkoutSession>> GetAllByUserIdAsync(Guid userId)
    {
        return await db.WorkoutSessions
            .AsNoTracking()
            .Include(x => x.WorkoutTemplate)
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.StartedAt)
            .ToListAsync();
    }
}