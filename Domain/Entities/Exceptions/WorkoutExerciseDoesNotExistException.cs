namespace Domain.Entities.Exceptions;

public class WorkoutExerciseDoesNotExistException(string message) : DomainException(message)
{
}
