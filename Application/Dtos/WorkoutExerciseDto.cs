namespace Application.Dtos;

public sealed record WorkoutExerciseDto(
    Guid Id,
    Guid ExerciseId,
    string ExerciseName,
    int Sets,
    int Reps
);