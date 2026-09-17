using Domain.Entities.Exceptions;

namespace Domain.Entities.Workout;

public class WorkoutTemplate : Entity
{
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public int RestInMinutes { get; private set; }

    public ICollection<WorkoutExercise> WorkoutExercises { get; private set; }

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
        WorkoutExercises.Add(workoutExercise);
    }

    public void RemoveExercise(WorkoutExercise workoutExercise)
    {
        WorkoutExercises.Remove(workoutExercise);
    }
}