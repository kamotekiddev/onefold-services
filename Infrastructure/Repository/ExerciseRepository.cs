using Application.Abstractions.Persistence;
using Domain.Entities.Workout;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository;

public class ExerciseRepository(AppDbContext db) : IExerciseRepository
{
    public void Add(Exercise exercise)
    {
        db.Exercises.Add(exercise);
    }

    public async Task<Exercise?> GetByNameAsync(string name)
    {
        return await db.Exercises.FirstOrDefaultAsync(e =>
            e.Name.ToLower() == name.ToLower());
    }

    public async Task<Exercise?> GetByIdAsync(Guid id)
    {
        return await db.Exercises.SingleOrDefaultAsync(e => e.Id == id);
    }

    public async Task<(IReadOnlyCollection<Exercise> Items, int TotalCount)> SearchAsync(
        Guid userId, string name, int offset, int pageSize)
    {
        // Treat LIKE metacharacters as literal search text, including the escape character itself.
        var escapedName = name.Replace("\\", "\\\\").Replace("%", "\\%").Replace("_", "\\_");
        var pattern = $"%{escapedName}%";
        var query = db.Exercises.AsNoTracking().Where(exercise =>
            exercise.IsActive &&
            (exercise.UserId == null || exercise.UserId == userId) &&
            EF.Functions.ILike(exercise.Name, pattern, "\\"));

        var totalCount = await query.CountAsync();
        var items = await query.OrderBy(exercise => exercise.Name).ThenBy(exercise => exercise.Id)
            .Skip(offset).Take(pageSize).ToListAsync();
        return (items, totalCount);
    }

    public async Task<IReadOnlyCollection<Exercise>> GetAvailableByIdsAsync(Guid userId, ICollection<Guid> ids)
    {
        return await db.Exercises.Where(exercise => ids.Contains(exercise.Id)).ToListAsync();
    }
}
