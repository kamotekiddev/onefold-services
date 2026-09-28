namespace Application.Features.Workout.WorkoutTemplateModule.CreateWorkoutTemplate;

public record CreateWorkoutTemplateRequest(
    string Name,
    string? Description,
    int RestInMinutes,
    IReadOnlyCollection<WorkoutItem> Exercises);