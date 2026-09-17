namespace Domain.Entities.Exceptions;

public class WorkoutExerciseAlreadyExistsException(string message) : Exception(message)
{
}