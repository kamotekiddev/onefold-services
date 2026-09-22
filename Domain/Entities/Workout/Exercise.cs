using Domain.Entities.Exceptions;

namespace Domain.Entities.Workout;

public class Exercise : Entity
{
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }

    public Guid? UserId { get; private set; }
    public User? User { get; init; }

    public ICollection<WorkoutTemplateExercise> WorkoutExercises { get; init; } = [];

    private Exercise()
    {
    }

    public static Exercise Create(string name, string? description = null)
    {
        return new Exercise()
        {
            Name = name,
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