using Domain.Entities.Exceptions;

namespace Domain.Entities.Workout;

public class WorkoutTemplate : Entity
{
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public int RestInMinutes { get; private set; }

    public IReadOnlyCollection<WorkoutExercise> WorkoutExercises =>
        _workoutExercises.AsReadOnly();

    private readonly List<WorkoutExercise> _workoutExercises = [];

    private WorkoutTemplate()
    {
    }

    public static WorkoutTemplate Create(string name, int restInMinutes)
    {
        if (restInMinutes <= 0) throw new DomainException("Invalid rest in minutes.");

        return new WorkoutTemplate()
        {
            Name = name,
            RestInMinutes = restInMinutes
        };
    }

    public void AddExercise(WorkoutExercise workoutExercise)
    {
        if (_workoutExercises.Any(e => e.ExerciseId == workoutExercise.ExerciseId))
            throw new WorkoutExerciseAlreadyExistsException("Workout exercise already added in this template.");

        _workoutExercises.Add(workoutExercise);
    }

    public void RemoveExercise(WorkoutExercise workoutExercise)
    {
        if (!_workoutExercises.Contains(workoutExercise))
            throw new WorkoutExerciseDoesNotExistException("Workout exercise does not exist.");
        _workoutExercises.Remove(workoutExercise);
    }
}