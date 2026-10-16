namespace Application.Dtos;

public sealed record WorkoutTemplateListItemDto(
    Guid Id,
    string Name,
    string? Description,
    int RestInMinutes,
    IReadOnlyCollection<DayOfWeek> ScheduledDays);
