namespace Application.Features.Workout.Template;

public record WorkoutItem(Guid ExerciseId, int SetCount, int TargetReps, int RestPerSetInSeconds, int OrderIdx);