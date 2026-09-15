using Domain.Entities.Workout;

namespace Application.Abstractions.Persistence;

public interface IExerciseRepository
{
    void Add(Exercise exercise);
    Task<Exercise?> GetByNameAsync(string name);
}