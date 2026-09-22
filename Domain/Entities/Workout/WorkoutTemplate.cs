using Domain.Entities.Exceptions;

namespace Domain.Entities.Workout;

public class WorkoutTemplate : Entity
{
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public int RestInMinutes { get; private set; }

    public Guid UserId { get; init; }
    public User User { get; init; }

    public IReadOnlyCollection<WorkoutTemplateExercise> WorkoutExercises =>
        _workoutExercises.AsReadOnly();

    private readonly List<WorkoutTemplateExercise> _workoutExercises = [];

    private WorkoutTemplate()
    {
    }

    public static WorkoutTemplate Create(Guid userId, string name, int restInMinutes, string? description = null)
    {
        if (userId == Guid.Empty) throw new DomainException("Invalid userId value.");
        if (restInMinutes <= 0) throw new DomainException("Invalid rest in minutes.");

        return new WorkoutTemplate()
        {
            UserId = userId,
            Name = name,
            RestInMinutes = restInMinutes,
            Description = description
        };
    }

    public void AddExercise(WorkoutTemplateExercise workoutTemplateExercise)
    {
        if (_workoutExercises.Any(e => e.ExerciseId == workoutTemplateExercise.ExerciseId))
            throw new WorkoutExerciseAlreadyExistsException("Workout exercise already added in this template.");

        _workoutExercises.Add(workoutTemplateExercise);
    }

    public void RemoveExercise(WorkoutTemplateExercise workoutTemplateExercise)
    {
        if (!_workoutExercises.Contains(workoutTemplateExercise))
            throw new WorkoutExerciseDoesNotExistException("Workout exercise does not exist.");
        _workoutExercises.Remove(workoutTemplateExercise);
    }
}