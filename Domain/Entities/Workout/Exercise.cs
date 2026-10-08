using Domain.Entities.Exceptions;
using Domain.Entities.Workout.Template;

namespace Domain.Entities.Workout;

public enum ExerciseType
{
    Bodyweight,
    Weighted
}

public class Exercise : Entity
{
    public string Name { get; private set; }
    public ExerciseType Type { get; private set; }
    public string? Description { get; private set; }


    public bool IsActive { get; private set; }

    public Guid? UserId { get; private set; }
    public User? User { get; init; }

    public ICollection<WorkoutTemplateExercise> WorkoutExercises { get; init; } = [];

    private Exercise()
    {
    }

    public static Exercise Create(string name, ExerciseType type, string? description = null)
    {
        return new Exercise()
        {
            Name = name,
            Type = type,
            Description = description
        };
    }

    public void AttachToUser(Guid userId)
    {
        if (userId == Guid.Empty) throw new DomainException("Invalid userId value.");
        UserId = userId;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}