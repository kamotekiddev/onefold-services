using Domain.Entities.Workout.Session;

namespace Application.Dtos;

public sealed record WorkoutSetDto(
    Guid Id,
    int SetNumber,
    int Reps,
    decimal? Weight,
    WeightUnit? Unit
);