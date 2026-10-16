namespace Domain.Entities.Exceptions;

public class WorkoutSessionExerciseDoesNotExistsException(string message) : DomainException(message)
{
}
