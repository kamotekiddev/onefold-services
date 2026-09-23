namespace Domain.Entities.Workout;

public class WorkoutSessionExercise : Entity
{
    public Guid SessionId { get; private set; }
    public Guid ExerciseId { get; private set; }
    public int SetCount { get; private set; }
    public int TargetReps { get; private set; }
    public int RestInSeconds { get; private set; }
    public int OrderIndex { get; private set; }

    public WorkoutSession Session { get; init; } = null!;
    public Exercise Exercise { get; init; } = null!;

    private readonly List<WorkoutSet> _sets = [];
    public IReadOnlyCollection<WorkoutSet> Sets => _sets.AsReadOnly();
}