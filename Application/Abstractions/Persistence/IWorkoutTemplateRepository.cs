using Domain.Entities.Workout;

namespace Application.Abstractions.Persistence;

public interface IWorkoutTemplateRepository
{
     void Add(WorkoutTemplate template);
    Task<WorkoutTemplate?> GetByNameAsync(string name);
    Task<bool> CheckUserOwnedByName(Guid userId, string name);
}