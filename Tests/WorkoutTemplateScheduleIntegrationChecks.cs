using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Domain.Entities;
using Domain.Entities.Workout;
using Infrastructure.Database;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using Onefold.Controllers.Workout;
using Onefold.Filters;

internal static class WorkoutTemplateScheduleIntegrationChecks
{
    public static async Task RunAsync(Action<bool, string> check)
    {
        var connectionString = Environment.GetEnvironmentVariable("ONEFOLD_TEST_CONNECTION_STRING");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            Console.WriteLine("Skipped PostgreSQL/HTTP schedule and exercise search checks: ONEFOLD_TEST_CONNECTION_STRING is not set.");
            return;
        }

        var connection = new NpgsqlConnectionStringBuilder(connectionString);
        if (connection.Host is not ("127.0.0.1" or "localhost") || connection.Database != "onefold_schedule_test")
            throw new InvalidOperationException("Schedule integration checks require a local onefold_schedule_test database.");

        var options = new DbContextOptionsBuilder<AppDbContext>().UseNpgsql(connectionString).Options;
        var owner = User.Create("schedule-owner@example.test");
        var otherOwner = User.Create("schedule-other@example.test");
        var legacyTemplateId = Guid.NewGuid();
        var exercise = Exercise.Create("Schedule test push-up", ExerciseType.Bodyweight);
        exercise.Activate();

        await using (var db = new AppDbContext(options))
        {
            if ((await db.Database.GetAppliedMigrationsAsync()).Any())
                throw new InvalidOperationException("Use a fresh disposable database for migration checks.");

            await db.GetService<IMigrator>().MigrateAsync("20261008080646_InitialCreate");
            db.Users.AddRange(owner, otherOwner);
            db.Exercises.Add(exercise);
            await db.SaveChangesAsync();
            await db.Database.ExecuteSqlInterpolatedAsync($"""
                INSERT INTO "WorkoutTemplates" ("Id", "Name", "RestInMinutes", "UserId", "CreatedAt")
                VALUES ({legacyTemplateId}, {"Legacy workout"}, {2}, {owner.Id}, {DateTimeOffset.UtcNow})
                """);

            await db.Database.MigrateAsync();
            var legacy = await db.WorkoutTemplates.SingleAsync(template => template.Id == legacyTemplateId);
            check(legacy.ScheduledDays.Count == 0, "migration leaves existing templates unscheduled");
            check(!db.Database.HasPendingModelChanges(), "migration snapshot matches the runtime model");
        }

        // Exercise the real controllers, handlers, JWT authentication, validation and PostgreSQL repositories.
        const string signingKey = "onefold-schedule-tests-only-signing-key-2026";
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions { EnvironmentName = "Production" });
        builder.Logging.ClearProviders();
        builder.WebHost.UseUrls("http://127.0.0.1:0");
        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ConnectionStrings:OnefoldDb"] = connectionString,
            ["Jwt:Issuer"] = "schedule-tests",
            ["Jwt:Audience"] = "schedule-tests",
            ["Jwt:SecretKey"] = signingKey
        });
        builder.Services.AddControllers(mvc => mvc.Filters.Add<ExceptionFilter>())
            .AddApplicationPart(typeof(TemplateController).Assembly)
            .AddJsonOptions(json => json.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
        builder.Services.Configure<ApiBehaviorOptions>(api =>
            api.InvalidModelStateResponseFactory = context => ErrorResponses.Validation(context.HttpContext, context.ModelState));
        Application.Application.AddApplication(builder.Services);
        Infrastructure.Infrastructure.AddInfrastructure(builder.Services, builder.Configuration);

        await using var app = builder.Build();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        await app.StartAsync();

        try
        {
            using var client = new HttpClient { BaseAddress = new Uri(app.Urls.Single()) };
            const string templatesUrl = "/api/workout/Template";
            var legacyScheduleUrl = $"{templatesUrl}/{legacyTemplateId}/schedule";
            using (var unauthorizedList = await client.GetAsync(templatesUrl))
                check(unauthorizedList.StatusCode == HttpStatusCode.Unauthorized, "template list requires authentication");
            using (var unauthorizedUpdate = await client.PutAsJsonAsync(legacyScheduleUrl, new { scheduledDays = new[] { "Monday" } }))
                check(unauthorizedUpdate.StatusCode == HttpStatusCode.Unauthorized, "schedule update requires authentication");

            void SignIn(Guid userId)
            {
                var token = new JwtSecurityToken("schedule-tests", "schedule-tests",
                    [new Claim(JwtRegisteredClaimNames.Sub, userId.ToString())],
                    expires: DateTime.UtcNow.AddMinutes(5),
                    signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
                        SecurityAlgorithms.HmacSha256));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",
                    new JwtSecurityTokenHandler().WriteToken(token));
            }

            async Task<Guid> CreateTemplate(string name, string[]? days, bool omitSchedule = false)
            {
                var body = new Dictionary<string, object?>
                {
                    ["name"] = name,
                    ["restInMinutes"] = 2,
                    ["exercises"] = new[] { new { exerciseId = exercise.Id, targetSet = 3, targetReps = 10, restInSeconds = 60, sortIndex = 0 } }
                };
                if (!omitSchedule)
                    body["scheduledDays"] = days;
                using var response = await client.PostAsJsonAsync(templatesUrl, body);
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadFromJsonAsync<JsonElement>();
                return json.GetProperty("templateId").GetGuid();
            }

            async Task<JsonElement> Get(string url)
            {
                using var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<JsonElement>();
            }

            SignIn(owner.Id);
            var mwf = await CreateTemplate("A MWF", ["Friday", "Monday", "Wednesday"]);
            var overlap = await CreateTemplate("B Monday", ["Monday"]);
            var omitted = await CreateTemplate("C No schedule", null, omitSchedule: true);
            var nullSchedule = await CreateTemplate("D Null schedule", null);
            var empty = await CreateTemplate("E Empty schedule", []);
            check(Days(await Get($"{templatesUrl}/{mwf}")).SequenceEqual(["Monday", "Wednesday", "Friday"]),
                "create and detail HTTP endpoints round-trip weekday names in order");
            foreach (var id in new[] { omitted, nullSchedule, empty })
                check(Days(await Get($"{templatesUrl}/{id}")).Length == 0, "optional creation schedule defaults to empty");

            SignIn(otherOwner.Id);
            var foreignTemplate = await CreateTemplate("Other owner Monday", ["Monday"]);
            using (var forbidden = await client.PutAsJsonAsync(legacyScheduleUrl, new { scheduledDays = new[] { "Sunday" } }))
                check(forbidden.StatusCode == HttpStatusCode.Forbidden, "schedule update enforces ownership over HTTP");

            SignIn(owner.Id);
            var monday = (await Get($"{templatesUrl}?day=Monday")).EnumerateArray().ToArray();
            check(monday.Select(row => row.GetProperty("id").GetGuid()).SequenceEqual([mwf, overlap]),
                "weekday filter returns multiple matching templates in name order and excludes other owners");
            var all = (await Get(templatesUrl)).EnumerateArray().ToArray();
            check(all.Length == 6 && all.All(row => row.GetProperty("id").GetGuid() != foreignTemplate),
                "unfiltered list includes scheduled and unscheduled templates belonging only to the current user");
            check((await Get($"{templatesUrl}?day=Sunday")).GetArrayLength() == 0, "unmatched weekday returns empty array");

            foreach (var invalidBody in new[]
                     {
                         "{}", "{\"scheduledDays\":null}", "{\"scheduledDays\":[\"Monday\",\"Monday\"]}",
                         "{\"scheduledDays\":[7]}", "{\"scheduledDays\":[\"Funday\"]}"
                     })
            {
                using var body = new StringContent(invalidBody, Encoding.UTF8, "application/json");
                using var response = await client.PutAsync(legacyScheduleUrl, body);
                check(response.StatusCode == HttpStatusCode.BadRequest, "invalid schedule returns HTTP 400");
                check(response.Content.Headers.ContentType?.MediaType == "application/problem+json",
                    "invalid schedule uses the existing problem response format");
            }
            foreach (var invalidDay in new[] { "7", "Funday" })
            {
                using var response = await client.GetAsync($"{templatesUrl}?day={invalidDay}");
                check(response.StatusCode == HttpStatusCode.BadRequest, "invalid weekday query returns HTTP 400");
            }
            using (var missing = await client.PutAsJsonAsync($"{templatesUrl}/{Guid.NewGuid()}/schedule", new { scheduledDays = Array.Empty<string>() }))
                check(missing.StatusCode == HttpStatusCode.NotFound, "missing template schedule returns HTTP 404");
            check(Days(await Get($"{templatesUrl}/{legacyTemplateId}")).Length == 0,
                "rejected schedule updates leave stored data unchanged");

            foreach (var days in new[]
                     {
                         new[] { "Tuesday", "Thursday", "Friday" },
                         new[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" },
                         Array.Empty<string>()
                     })
            {
                using var response = await client.PutAsJsonAsync(legacyScheduleUrl, new { scheduledDays = days });
                response.EnsureSuccessStatusCode();
                var updated = await response.Content.ReadFromJsonAsync<JsonElement>();
                check(updated.GetProperty("templateId").GetGuid() == legacyTemplateId && Days(updated).SequenceEqual(days),
                    "schedule replacement returns the current schedule");
                check(Days(await Get($"{templatesUrl}/{legacyTemplateId}")).SequenceEqual(days),
                    "schedule replacement and clearing survive a fresh database read");
            }

            await using var verificationDb = new AppDbContext(options);
            var repository = new WorkoutTemplateRepository(verificationDb);
            await repository.GetAllByUserIdAsync(owner.Id, DayOfWeek.Monday);
            check(!verificationDb.ChangeTracker.Entries().Any(), "template listing does not track entities");

            await ExerciseSearchIntegrationChecks.RunAsync(options, client, SignIn, owner.Id, otherOwner.Id, check);
        }
        finally
        {
            await app.StopAsync();
        }
    }

    private static string[] Days(JsonElement template)
        => template.GetProperty("scheduledDays").EnumerateArray().Select(day => day.GetString()!).ToArray();
}
