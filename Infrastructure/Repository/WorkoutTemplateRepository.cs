using Application.Abstractions.Persistence;
using Domain.Entities.Workout;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository;

public class WorkoutTemplateRepository(AppDbContext db) : IWorkoutTemplateRepository
{
    public void Add(WorkoutTemplate template)
    {
        db.WorkoutTemplates.Add(template);
    }

    public async Task<WorkoutTemplate?> GetByNameAsync(string name)
    {
        return await db.WorkoutTemplates.FirstOrDefaultAsync(wt => wt.Name == name);
    }

    public async Task<bool> CheckUserOwnedByName(Guid userId, string name)
    {
        var existingTemplate =
            await db.WorkoutTemplates.FirstOrDefaultAsync(wt => wt.Name == name && wt.UserId == userId);

        return existingTemplate is not null;
    }
}