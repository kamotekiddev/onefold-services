namespace Application.Features.Workout.CreateWorkoutTemplate;

public record WorkoutItem(Guid ExerciseId, int TargetReps, int RestPerSetInSeconds, int OrderIdx);