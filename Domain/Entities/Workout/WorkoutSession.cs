namespace Domain.Entities.Workout;

public enum WorkoutSessionStatus
{
    InProgress,
    Completed,
    Canceled
}

public class WorkoutSession : Entity
{
    public Guid WorkoutTemplateId { get; init; }
    public WorkoutSessionStatus Status { get; private set; }

    public ICollection<WorkoutSessionExercise> Exercises { get; private set; } = [];
}