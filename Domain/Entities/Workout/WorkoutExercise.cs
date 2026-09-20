using Domain.Entities.Exceptions;

namespace Domain.Entities.Workout;

public class WorkoutExercise : Entity
{
    public required Guid ExerciseId { get; init; }
    public required Guid WorkoutTemplateId { get; init; }
    public int TargetReps { get; private set; }
    public int RestPerSetInSeconds { get; private set; }
    public int OrderIndex { get; private set; }

    public Exercise Exercise { get; init; }
    public WorkoutTemplate WorkoutTemplate { get; init; }

    private WorkoutExercise()
    {
    }

    public static WorkoutExercise Create(
        Guid workoutTemplateId,
        Guid exerciseId,
        int targetReps,
        int restPerSetInSeconds,
        int orderIdx)
    {
        if (exerciseId == Guid.Empty)
            throw new DomainException("Invalid exerciseId value.");

        if (workoutTemplateId == Guid.Empty)
            throw new DomainException("Invalid workoutTemplateId value.");

        if (targetReps <= 0)
            throw new DomainException("Invalid targetReps value.");

        if (restPerSetInSeconds <= 0)
            throw new DomainException("Invalid restPerSet value.");

        return new WorkoutExercise()
        {
            WorkoutTemplateId = workoutTemplateId,
            ExerciseId = exerciseId,
            TargetReps = targetReps,
            RestPerSetInSeconds = restPerSetInSeconds,
            OrderIndex = orderIdx
        };
    }
}