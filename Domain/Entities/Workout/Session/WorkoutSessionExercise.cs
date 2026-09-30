using Domain.Entities.Exceptions;

namespace Domain.Entities.Workout.Session;

public class WorkoutSessionExercise : Entity
{
    public Guid SessionId { get; private set; }
    public Guid ExerciseId { get; private set; }
    public int TargetSet { get; private set; }
    public int TargetReps { get; private set; }
    public int RestInSeconds { get; private set; }
    public int SortIndex { get; private set; }

    public WorkoutSession Session { get; init; } = null!;
    public Exercise Exercise { get; init; } = null!;

    private readonly List<WorkoutSet> _sets = [];
    public IReadOnlyCollection<WorkoutSet> Sets => _sets.AsReadOnly();

    private WorkoutSessionExercise()
    {
    }

    public static WorkoutSessionExercise Create(
        Guid sessionId,
        Guid exerciseId,
        int targetSets,
        int targetReps,
        int restInSeconds,
        int sortIndex)
    {
        if (sessionId == Guid.Empty)
            throw new DomainException("Invalid session ID.");

        if (exerciseId == Guid.Empty)
            throw new DomainException("Invalid exercise ID.");

        if (targetSets <= 0)
            throw new DomainException("Target sets must be greater than zero.");

        if (targetReps <= 0)
            throw new DomainException("Target reps must be greater than zero.");

        if (restInSeconds < 0)
            throw new DomainException("Rest in seconds cannot be negative.");

        if (sortIndex < 0)
            throw new DomainException("Sort index cannot be negative.");

        return new WorkoutSessionExercise
        {
            SessionId = sessionId,
            ExerciseId = exerciseId,
            TargetSet = targetSets,
            TargetReps = targetReps,
            RestInSeconds = restInSeconds,
            SortIndex = sortIndex
        };
    }

    public void AddSet(int setNumber, int reps, decimal? weight, WeightUnit? unit)
    {
        if (_sets.Any(x => x.SetNumber == setNumber))
            throw new WorkoutSetAlreadyExistsException();

        var set = WorkoutSet.Create(
            Id,
            setNumber,
            reps,
            weight,
            unit);

        _sets.Add(set);
    }
}