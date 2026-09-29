namespace Application.Dtos;

public sealed record WorkoutSessionListItemDto(
    Guid Id,
    Guid WorkoutTemplateId,
    string WorkoutTemplateName,
    DateTimeOffset StartedAt,
    DateTimeOffset? CompletedAt
);