namespace Domain.Entities.Exceptions;

public class WorkoutSessionExerciseDoesNotExistsException(string message) : Exception(message)
{
}