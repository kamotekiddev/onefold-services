using Domain.Entities.Exceptions;

namespace Domain.Entities.Workout.Session;

public enum WeightUnit
{
    Kilograms,
    Pounds
}

public class WorkoutSet : Entity
{
    public Guid WorkoutSessionExerciseId { get; private set; }

    public int SetNumber { get; private set; }
    public int Reps { get; private set; }
    public decimal? Weight { get; private set; }
    public WeightUnit? Unit { get; private set; }

    public WorkoutSessionExercise WorkoutSessionExercise { get; init; } = null!;

    private WorkoutSet()
    {
    }

    public static WorkoutSet Create(
        Guid workoutSessionExerciseId,
        int setNumber,
        int reps,
        decimal? weight,
        WeightUnit? unit)
    {
        if (workoutSessionExerciseId == Guid.Empty)
            throw new DomainException("Invalid workout session exercise ID.");

        if (setNumber <= 0)
            throw new DomainException("Set number must be greater than zero.");

        if (reps <= 0)
            throw new DomainException("Reps must be greater than zero.");

        if (weight < 0)
            throw new DomainException("Weight cannot be negative.");

        if (unit.HasValue && !Enum.IsDefined(unit.Value))
            throw new DomainException("Invalid weight unit.");

        return new WorkoutSet
        {
            WorkoutSessionExerciseId = workoutSessionExerciseId,
            SetNumber = setNumber,
            Reps = reps,
            Weight = weight,
            Unit = unit
        };
    }
}