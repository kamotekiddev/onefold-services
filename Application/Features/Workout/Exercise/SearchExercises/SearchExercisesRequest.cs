namespace Application.Features.Workout.Exercise.SearchExercises;

public sealed record SearchExercisesRequest(string? Name = null, int Page = 1, int PageSize = 20);
