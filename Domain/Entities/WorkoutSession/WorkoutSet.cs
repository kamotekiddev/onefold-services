using Domain.Entities.Workout;

namespace Domain.Entities.WorkoutSession;

public class WorkoutSet : Entity
{
    public Guid WorkoutSessionExerciseId { get; private set; }

    public int SetNumber { get; private set; }
    public int Reps { get; private set; }
    public decimal? Weight { get; private set; }

    public WorkoutSessionExercise WorkoutSessionExercise { get; init; } = null!;
}