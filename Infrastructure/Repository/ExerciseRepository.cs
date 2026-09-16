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

    public async Task<Exercise?> GetById(Guid id)
    {
        return await db.Exercises.SingleOrDefaultAsync(e => e.Id == id);
    }
}