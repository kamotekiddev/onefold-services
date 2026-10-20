using Application.Dtos;

namespace Application.Features.Workout.Exercise.SearchExercises;

public sealed record SearchExercisesResponse(
    IReadOnlyCollection<ExerciseDto> Items, int Page, int PageSize, int TotalCount);
