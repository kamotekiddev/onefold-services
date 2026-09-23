namespace Application.Features.Workout.WorkoutTemplateModule;

public record WorkoutItem(Guid ExerciseId, int SetCount, int TargetReps, int RestPerSetInSeconds, int OrderIdx);