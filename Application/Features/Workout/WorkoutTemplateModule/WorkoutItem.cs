namespace Application.Features.Workout.WorkoutTemplateModule;

public record WorkoutItem(Guid ExerciseId, int TargetReps, int RestPerSetInSeconds, int OrderIdx);