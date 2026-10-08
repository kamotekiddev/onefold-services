using Domain.Entities.Workout.Session;

namespace Application.Features.Workout.Template;

public record WorkoutItem(
    Guid ExerciseId,
    int TargetSet,
    int TargetReps,
    int RestInSeconds,
    int SortIndex,
    WeightUnit? WeightUnit = null);