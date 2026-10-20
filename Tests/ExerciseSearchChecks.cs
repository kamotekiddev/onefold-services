using Application.Abstractions.Authentication;
using Application.Abstractions.Persistence;
using Application.Features.Workout.Exercise.SearchExercises;
using Domain.Entities.Workout;
using FluentValidation;

internal static class ExerciseSearchChecks
{
    public static async Task RunAsync(Action<bool, string> check)
    {
        var validator = new SearchExercisesRequestValidator();
        var request = new SearchExercisesRequest("press");
        check(request.Page == 1 && request.PageSize == 20, "exercise search defaults to page 1 with 20 items");
        foreach (var name in new string?[] { null, "", " \t ", new('x', 256) })
            check(!validator.Validate(request with { Name = name }).IsValid,
                "exercise search rejects missing, blank and oversized names");
        check(validator.Validate(request with { Name = $"  {new string('x', 255)}  " }).IsValid,
            "exercise search applies the name length limit after trimming");
        foreach (var invalid in new[]
                 {
                     request with { Page = 0 }, request with { Page = -1 },
                     request with { PageSize = 0 }, request with { PageSize = -1 },
                     request with { PageSize = 101 }, request with { Page = int.MaxValue },
                     request with { Page = int.MinValue, PageSize = int.MinValue }
                 })
            check(!validator.Validate(invalid).IsValid, "exercise search rejects invalid pagination and offset overflow");
        check(validator.Validate(request with { PageSize = 100 }).IsValid &&
              validator.Validate(request with { Page = int.MaxValue, PageSize = 1 }).IsValid,
            "exercise search accepts maximum page size and a large representable offset");

        var userId = Guid.NewGuid();
        var repository = new SearchRepository();
        var handler = new SearchExercisesHandler(repository, new CurrentUser(userId), validator);
        var result = await handler.ExecuteAsync(new SearchExercisesRequest("  PrEsS  ", 3, 2));
        check(repository.LastQuery == (userId, "PrEsS", 4, 2),
            "exercise search passes current-user identity, trimmed name and pagination to the repository");
        check(result.Page == 3 && result.PageSize == 2 && result.TotalCount == 7 &&
              result.Items.Single().Id == repository.Exercise.Id &&
              result.Items.Single().Name == repository.Exercise.Name,
            "exercise search maps item DTOs and preserves pagination metadata");
        try
        {
            await handler.ExecuteAsync(new SearchExercisesRequest(" "));
            throw new Exception("Expected invalid exercise search to fail.");
        }
        catch (ValidationException)
        {
            check(repository.Calls == 1, "invalid exercise search fails before querying persistence");
        }
    }

    private sealed class CurrentUser(Guid userId) : ICurrentUser
    {
        public Guid UserId => userId;
    }

    private sealed class SearchRepository : IExerciseRepository
    {
        public Exercise Exercise { get; } = Exercise.Create("Bench Press", ExerciseType.Weighted);
        public (Guid UserId, string Name, int Offset, int PageSize) LastQuery { get; private set; }
        public int Calls { get; private set; }

        public Task<(IReadOnlyCollection<Exercise> Items, int TotalCount)> SearchAsync(
            Guid userId, string name, int offset, int pageSize)
        {
            LastQuery = (userId, name, offset, pageSize);
            Calls++;
            return Task.FromResult<(IReadOnlyCollection<Exercise>, int)>(([Exercise], 7));
        }

        public void Add(Exercise exercise) => throw new NotSupportedException();
        public Task<Exercise?> GetByNameAsync(string name) => throw new NotSupportedException();
        public Task<Exercise?> GetByIdAsync(Guid id) => throw new NotSupportedException();
        public Task<IReadOnlyCollection<Exercise>> GetAvailableByIdsAsync(Guid userId, ICollection<Guid> ids)
            => throw new NotSupportedException();
    }
}
