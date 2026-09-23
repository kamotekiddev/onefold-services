using Domain.Entities.Exceptions;

namespace Domain.Entities.Workout;

public class WorkoutTemplateExercise : Entity
{
    public required Guid ExerciseId { get; init; }
    public required Guid WorkoutTemplateId { get; init; }
    public int TargetRepsPerSet { get; private set; }
    public int SetCount { get; private set; }
    public int RestPerSetInSeconds { get; private set; }
    public int OrderIndex { get; private set; }

    public Exercise Exercise { get; init; }
    public WorkoutTemplate WorkoutTemplate { get; init; }

    private WorkoutTemplateExercise()
    {
    }

    public static WorkoutTemplateExercise Create(
        Guid workoutTemplateId,
        Guid exerciseId,
        int setCount,
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

        if (setCount <= 0) throw new DomainException("Invalid set value.");

        return new WorkoutTemplateExercise()
        {
            WorkoutTemplateId = workoutTemplateId,
            ExerciseId = exerciseId,
            SetCount = setCount,
            TargetRepsPerSet = targetReps,
            RestPerSetInSeconds = restPerSetInSeconds,
            OrderIndex = orderIdx
        };
    }

    public void Update(int setCount, int targetReps, int restPerSetInSeconds, int orderIdx)
    {
        SetCount = setCount;
        TargetRepsPerSet = targetReps;
        RestPerSetInSeconds = restPerSetInSeconds;
        OrderIndex = orderIdx;
    }
}