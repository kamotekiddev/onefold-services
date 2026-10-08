namespace Application.Features.Workout.Template.UpdateWorkoutTemplate;

public record UpdateWorkoutTemplateRequest(
    string Name,
    string? Description,
    int RestInMinutes,
    IReadOnlyCollection<WorkoutItem> Exercises);