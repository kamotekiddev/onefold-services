using Domain.Entities.Workout;

namespace Application.Features.Workout.Exercise.CreateExercise;

public record CreateExerciseRequest(string Name, ExerciseType Type, string? Description);