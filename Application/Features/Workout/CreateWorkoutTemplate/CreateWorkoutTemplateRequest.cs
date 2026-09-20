namespace Application.Features.Workout.CreateWorkoutTemplate;

public record CreateWorkoutTemplateRequest(
    string Name,
    string? Description,
    int RestInMinutes,
    IReadOnlyCollection<WorkoutItem> Exercises);