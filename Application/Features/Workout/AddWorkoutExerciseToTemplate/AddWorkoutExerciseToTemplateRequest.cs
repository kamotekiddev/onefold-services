namespace Application.Features.Workout.AddWorkoutExerciseToTemplate;

public record AddWorkoutExerciseToTemplateRequest(
    Guid ExerciseId,
    int TargetReps,
    int RestPerSetInSeconds,
    int OrderIdx
);