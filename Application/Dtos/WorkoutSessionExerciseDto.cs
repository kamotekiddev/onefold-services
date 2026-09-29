namespace Application.Dtos;

public sealed record WorkoutSessionExerciseDto(
    Guid Id,
    Guid ExerciseId,
    string ExerciseName,
    int TargetSet,
    int TargetReps,
    int RestInSeconds,
    int SortIndex,
    IReadOnlyCollection<WorkoutSetDto> Sets
);