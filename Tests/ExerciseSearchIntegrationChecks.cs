using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Domain.Entities.Workout;
using Infrastructure.Database;
using Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;

internal static class ExerciseSearchIntegrationChecks
{
    public static async Task RunAsync(
        DbContextOptions<AppDbContext> options, HttpClient client, Action<Guid> signIn,
        Guid ownerId, Guid otherOwnerId, Action<bool, string> check)
    {
        const string url = "/api/workout/Exercise/search";
        var global = Create("Search fixture Alpha Press");
        var owned = Create("Search fixture Beta Press", ownerId);
        var duplicate = Create(owned.Name);
        var inactiveGlobal = Create("Search fixture Inactive Press", active: false);
        var inactiveOwned = Create("Search fixture Inactive Press", ownerId, active: false);
        var foreign = Create("Search fixture Other Press", otherOwnerId);
        var foreignInactive = Create("Search fixture Other inactive Press", otherOwnerId, active: false);
        var literal = Create(@"Search literal 50%_Press\End");
        var decoy = Create("Search literal 500xPressEnd");
        await using (var db = new AppDbContext(options))
        {
            db.Exercises.AddRange(global, owned, duplicate, inactiveGlobal, inactiveOwned, foreign,
                foreignInactive, literal, decoy);
            await db.SaveChangesAsync();
        }

        client.DefaultRequestHeaders.Authorization = null;
        using (var unauthorized = await client.GetAsync($"{url}?name=press"))
            check(unauthorized.StatusCode == HttpStatusCode.Unauthorized, "exercise search requires authentication");
        signIn(ownerId);

        async Task<JsonElement> Get(string query)
        {
            using var response = await client.GetAsync($"{url}?{query}");
            check(response.StatusCode == HttpStatusCode.OK, "valid exercise search returns HTTP 200");
            return await response.Content.ReadFromJsonAsync<JsonElement>();
        }

        var result = await Get("name=%20%20sEaRcH%20FiXtUrE%20%20");
        var items = result.GetProperty("items").EnumerateArray().ToArray();
        var duplicateIds = new[] { owned.Id, duplicate.Id }.Order().ToArray();
        var expected = new[] { global.Id }.Concat(duplicateIds).ToArray();
        check(items.Select(item => item.GetProperty("id").GetGuid()).SequenceEqual(expected),
            "search is trimmed, case-insensitive, ordered by name and ID, and retains duplicate names");
        check(result.GetProperty("totalCount").GetInt32() == 3 &&
              result.GetProperty("page").GetInt32() == 1 && result.GetProperty("pageSize").GetInt32() == 20,
            "search defaults and total count include only active global and current-user exercises");
        check(items.All(item => item.EnumerateObject().Select(property => property.Name).Order()
                .SequenceEqual(new[] { "id", "name" })),
            "exercise search items expose only the existing exercise DTO fields");

        var pageIds = new List<Guid>();
        for (var page = 1; page <= 3; page++)
        {
            var paged = await Get($"name=Search%20fixture&page={page}&pageSize=1");
            pageIds.Add(paged.GetProperty("items")[0].GetProperty("id").GetGuid());
            check(paged.GetProperty("totalCount").GetInt32() == 3 &&
                  paged.GetProperty("page").GetInt32() == page &&
                  paged.GetProperty("pageSize").GetInt32() == 1,
                "search pages preserve the full matching count and requested pagination");
        }
        check(pageIds.SequenceEqual(expected), "successive search pages are stable without duplicate or missing IDs");
        var beyond = await Get("name=Search%20fixture&page=4&pageSize=1");
        check(beyond.GetProperty("items").GetArrayLength() == 0 && beyond.GetProperty("totalCount").GetInt32() == 3,
            "page beyond search results returns empty items with the matching total");
        var empty = await Get("name=NoSuchExerciseFixture");
        check(empty.GetProperty("items").GetArrayLength() == 0 && empty.GetProperty("totalCount").GetInt32() == 0,
            "unmatched search returns empty items and zero count");

        foreach (var term in new[] { "%", "_", "\\", @"50%_pReSs\eNd" })
        {
            var escaped = await Get($"name={Uri.EscapeDataString(term)}");
            check(escaped.GetProperty("totalCount").GetInt32() == 1 &&
                  escaped.GetProperty("items")[0].GetProperty("id").GetGuid() == literal.Id,
                "exercise search treats wildcard and escape characters literally while matching substrings");
        }
        foreach (var (query, field) in new[]
                 {
                     ("", "Name"), ("name=", "Name"), ("name=%20%20", "Name"),
                     ($"name={new string('x', 256)}", "Name"),
                     ("name=press&page=0", "Page"), ("name=press&page=-1", "Page"),
                     ("name=press&pageSize=0", "PageSize"), ("name=press&pageSize=101", "PageSize"),
                     ("name=press&page=2147483647", "Page"),
                     ("name=press&page=abc", "Page"), ("name=press&pageSize=abc", "PageSize"),
                     ("name=press&page=2147483648", "Page")
                 })
        {
            using var response = await client.GetAsync($"{url}?{query}");
            check(response.StatusCode == HttpStatusCode.BadRequest &&
                  response.Content.Headers.ContentType?.MediaType == "application/problem+json",
                "invalid search parameters return HTTP 400 problem responses");
            var problem = await response.Content.ReadFromJsonAsync<JsonElement>();
            check(problem.GetProperty("errors").EnumerateObject()
                      .Any(property => property.Name.Equals(field, StringComparison.OrdinalIgnoreCase)) &&
                  problem.TryGetProperty("traceId", out _),
                "search validation includes the offending field and trace ID");
        }

        using (var detail = await client.GetAsync($"/api/workout/Exercise/{owned.Id}"))
        {
            check(detail.StatusCode == HttpStatusCode.OK, "exercise ID route remains available alongside search");
            var body = await detail.Content.ReadFromJsonAsync<JsonElement>();
            check(body.GetProperty("id").GetGuid() == owned.Id && body.GetProperty("name").GetString() == owned.Name,
                "existing exercise detail response is preserved");
        }

        signIn(otherOwnerId);
        var otherResults = await Get("name=Search%20fixture");
        var otherIds = otherResults.GetProperty("items").EnumerateArray()
            .Select(item => item.GetProperty("id").GetGuid()).ToHashSet();
        check(otherResults.GetProperty("totalCount").GetInt32() == 3 &&
              otherIds.SetEquals([global.Id, duplicate.Id, foreign.Id]),
            "exercise visibility and counts follow the authenticated user");
        signIn(ownerId);

        await using var verificationDb = new AppDbContext(options);
        await new ExerciseRepository(verificationDb).SearchAsync(ownerId, "Search fixture", 0, 20);
        check(!verificationDb.ChangeTracker.Entries().Any(), "exercise search does not track entities");
    }

    private static Exercise Create(string name, Guid? userId = null, bool active = true)
    {
        var exercise = Exercise.Create(name, ExerciseType.Weighted);
        if (userId.HasValue)
            exercise.AttachToUser(userId.Value);
        if (active)
            exercise.Activate();
        return exercise;
    }
}
