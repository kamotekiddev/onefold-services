using Domain.Entities.Workout;

namespace Application.Abstractions.Persistence;

public interface IWorkoutTemplateRepository
{
    void Add(WorkoutTemplate template);
    Task<WorkoutTemplate?> GetByNameAsync(string name);
    Task<bool> CheckUserOwnedByNameAsync(Guid userId, string name);
    Task<WorkoutTemplate?> GetByIdAsync(Guid templateId);
}