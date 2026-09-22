namespace Application.Dtos;

public sealed record WorkoutTemplateDto(
    Guid Id,
    Guid UserId,
    string Name,
    string? Description,
    int RestInMinutes,
    IReadOnlyCollection<WorkoutExerciseDto> WorkoutExercises
);