namespace Domain.Entities.Exceptions;

public class WorkoutSessionExerciseAlreadyExistsException(string message) : DomainException(message)
{
}
