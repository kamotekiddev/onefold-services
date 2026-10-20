using Application.Abstractions.Authentication;
using Application.Abstractions.Persistence;
using Application.Dtos;
using FluentValidation;

namespace Application.Features.Workout.Exercise.SearchExercises;

public sealed class SearchExercisesHandler(
    IExerciseRepository exerciseRepository,
    ICurrentUser currentUser,
    IValidator<SearchExercisesRequest> validator)
{
    public async Task<SearchExercisesResponse> ExecuteAsync(SearchExercisesRequest request)
    {
        await validator.ValidateAndThrowAsync(request);

        var offset = checked((request.Page - 1) * request.PageSize);
        var (exercises, totalCount) = await exerciseRepository.SearchAsync(
            currentUser.UserId, request.Name!.Trim(), offset, request.PageSize);

        return new SearchExercisesResponse(
            exercises.Select(exercise => new ExerciseDto(exercise.Id, exercise.Name)).ToList(),
            request.Page, request.PageSize, totalCount);
    }
}
