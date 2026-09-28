using Domain.Entities.Exceptions;
using Domain.Entities.Workout.Template;

namespace Domain.Entities.Workout.Session;

public class WorkoutSession : Entity
{
    public Guid WorkoutTemplateId { get; init; }
    public Guid UserId { get; init; }
    public DateTimeOffset StartedAt { get; init; }
    public DateTimeOffset CompletedAt { get; init; }

    public WorkoutTemplate WorkoutTemplate { get; init; }
    public User User { get; init; }
    public ICollection<WorkoutSessionExercise> Exercises => _exercises.AsReadOnly();

    private IList<WorkoutSessionExercise> _exercises = [];

    private WorkoutSession()
    {
    }

    public static WorkoutSession Create(
        Guid workoutTemplateId,
        Guid userId,
        DateTimeOffset startedAt,
        DateTimeOffset completedAt)
    {
        if (workoutTemplateId == Guid.Empty)
            throw new DomainException("Invalid workoutTemplateId value.");

        if (userId == Guid.Empty)
            throw new DomainException("Invalid userId value.");

        return new WorkoutSession()
        {
            WorkoutTemplateId = workoutTemplateId,
            UserId = userId,
            StartedAt = startedAt,
            CompletedAt = completedAt
        };
    }

    public void AddExercise(WorkoutSessionExercise exercise)
    {
        if (_exercises.Any(x => x.ExerciseId == exercise.ExerciseId))
            throw new WorkoutSessionExerciseAlreadyExistsException($"Exercise:{exercise.ExerciseId} already exists.");

        _exercises.Add(exercise);
    }

    public void RemoveExercise(WorkoutSessionExercise exercise)
    {
        if (!_exercises.Contains(exercise))
            throw new WorkoutSessionExerciseDoesNotExistsException($"Exercise:{exercise.ExerciseId}  does not exist.");

        _exercises.Add(exercise);
    }
}