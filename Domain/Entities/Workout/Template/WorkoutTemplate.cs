using Domain.Entities.Exceptions;
using Domain.Entities.Workout.Session;

namespace Domain.Entities.Workout.Template;

public class WorkoutTemplate : Entity
{
    public string Name { get; private set; }

    public string? Description { get; private set; }

    public int RestInMinutes { get; private set; }

    public IReadOnlyCollection<DayOfWeek> ScheduledDays => Array.AsReadOnly(_scheduledDays);

    public Guid UserId { get; init; }

    public User User { get; init; }

    public IReadOnlyCollection<WorkoutTemplateExercise> WorkoutExercises =>
        _workoutExercises.AsReadOnly();

    public ICollection<WorkoutSession> WorkoutSessions { get; init; } = [];

    private readonly List<WorkoutTemplateExercise> _workoutExercises = [];
    private DayOfWeek[] _scheduledDays = [];

    private WorkoutTemplate()
    {
    }

    public static WorkoutTemplate Create(Guid userId, string name, int restInMinutes, string? description = null,
        IReadOnlyCollection<DayOfWeek>? scheduledDays = null)
    {
        if (userId == Guid.Empty) throw new DomainException("Invalid userId value.");
        if (restInMinutes <= 0) throw new DomainException("Invalid rest in minutes.");

        var template = new WorkoutTemplate()
        {
            UserId = userId,
            Name = name,
            RestInMinutes = restInMinutes,
            Description = description
        };

        template.ReplaceSchedule(scheduledDays ?? []);
        return template;
    }

    public void ReplaceSchedule(IReadOnlyCollection<DayOfWeek> scheduledDays)
    {
        if (scheduledDays is null || scheduledDays.Any(day => !Enum.IsDefined(day)))
            throw new DomainException("Scheduled days must contain valid weekdays.");

        if (scheduledDays.Distinct().Count() != scheduledDays.Count)
            throw new DomainException("Each weekday can only be scheduled once.");

        _scheduledDays = scheduledDays.OrderBy(day => ((int)day + 6) % 7).ToArray();
    }

    public void AddExercise(WorkoutTemplateExercise workoutTemplateExercise)
    {
        if (_workoutExercises.Any(e => e.ExerciseId == workoutTemplateExercise.ExerciseId))
            throw new WorkoutExerciseAlreadyExistsException("Workout exercise already added in this template.");

        _workoutExercises.Add(workoutTemplateExercise);
    }

    public void RemoveExercise(WorkoutTemplateExercise workoutTemplateExercise)
    {
        if (!_workoutExercises.Contains(workoutTemplateExercise))
            throw new WorkoutExerciseDoesNotExistException("Workout exercise does not exist.");
        _workoutExercises.Remove(workoutTemplateExercise);
    }
}
