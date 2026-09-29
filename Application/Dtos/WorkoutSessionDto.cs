namespace Application.Dtos;

public sealed record WorkoutSessionDto(
    Guid Id,
    Guid WorkoutTemplateId,
    DateTimeOffset StartedAt,
    DateTimeOffset CompletedAt,
    IReadOnlyCollection<WorkoutSessionExerciseDto> Exercises
);