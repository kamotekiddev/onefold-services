namespace Domain.Entities.Exceptions;

public class WorkoutExerciseDoesNotExistException(string message) : Exception(message)
{
}