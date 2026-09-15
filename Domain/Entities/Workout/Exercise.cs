using Domain.Entities.Exceptions;

namespace Domain.Entities.Workout;

public class Exercise : Entity
{
    public string Name { get; private set; }
    public bool IsActive { get; private set; }

    public Guid? UserId { get; private set; }
    public User? User { get; init; }


    private Exercise()
    {
    }

    public static Exercise Create(string name)
    {
        return new Exercise()
        {
            Name = name
        };
    }

    public void AttachToUser(Guid userId)
    {
        if (userId == Guid.Empty) throw new InvalidIdException();
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