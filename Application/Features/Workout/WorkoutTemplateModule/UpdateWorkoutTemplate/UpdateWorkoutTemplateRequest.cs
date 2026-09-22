namespace Application.Features.Workout.WorkoutTemplateModule.UpdateWorkoutTemplate;

public record UpdateWorkoutTemplateRequest(
    string Name,
    string? Description,
    int RestInMinutes,
    IReadOnlyCollection<WorkoutItem> Exercises);