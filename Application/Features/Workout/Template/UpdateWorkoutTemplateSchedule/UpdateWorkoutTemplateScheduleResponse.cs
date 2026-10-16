namespace Application.Features.Workout.Template.UpdateWorkoutTemplateSchedule;

public sealed record UpdateWorkoutTemplateScheduleResponse(
    Guid TemplateId,
    IReadOnlyCollection<DayOfWeek> ScheduledDays);
