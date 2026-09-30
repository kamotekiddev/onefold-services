using Domain.Entities.Workout.Session;

namespace Application.Features.Workout.Session.SaveSession;

public sealed record SaveSessionRequest(
    Guid WorkoutTemplateId,
    DateTimeOffset StartedAt,
    DateTimeOffset CompletedAt,
    IReadOnlyCollection<SaveSessionExerciseRequest> Exercises
);

public sealed record SaveSessionExerciseRequest(
    Guid ExerciseId,
    int SortIndex,
    IReadOnlyCollection<SaveSessionWorkoutSetRequest> Sets
);

public sealed record SaveSessionWorkoutSetRequest(
    int SetNumber,
    int Reps,
    decimal? Weight,
    WeightUnit? Unit
);