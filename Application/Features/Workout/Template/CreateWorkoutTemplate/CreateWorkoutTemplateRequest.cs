namespace Application.Features.Workout.Template.CreateWorkoutTemplate;

public record CreateWorkoutTemplateRequest(
    string Name,
    string? Description,
    int RestInMinutes,
    IReadOnlyCollection<WorkoutItem> Exercises);