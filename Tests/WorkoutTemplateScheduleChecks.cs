using Application.Abstractions.Authentication;
using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using Application.Features.Workout.Template;
using Application.Features.Workout.Template.CreateWorkoutTemplate;
using Application.Features.Workout.Template.GetWorkoutTemplateById;
using Application.Features.Workout.Template.GetWorkoutTemplates;
using Application.Features.Workout.Template.UpdateWorkoutTemplateSchedule;
using Domain.Entities.Exceptions;
using Domain.Entities.Workout;
using Domain.Entities.Workout.Template;
using FluentValidation;
using Microsoft.Extensions.Logging.Abstractions;

internal static class WorkoutTemplateScheduleChecks
{
    public static async Task RunAsync(Action<bool, string> check)
    {
        var userId = Guid.NewGuid();
        var template = WorkoutTemplate.Create(userId, "Weekly workout", 2);
        check(template.ScheduledDays.Count == 0, "templates default to unscheduled");

        DayOfWeek[] selectedDays = [DayOfWeek.Friday, DayOfWeek.Monday, DayOfWeek.Wednesday];
        template.ReplaceSchedule(selectedDays);
        selectedDays[0] = DayOfWeek.Sunday;
        check(template.ScheduledDays.SequenceEqual([DayOfWeek.Monday, DayOfWeek.Wednesday, DayOfWeek.Friday]),
            "schedule is copied and ordered Monday to Sunday");
        check(((ICollection<DayOfWeek>)template.ScheduledDays).IsReadOnly,
            "schedule cannot be modified outside domain methods");

        ExpectDomainFailure(() => template.ReplaceSchedule([DayOfWeek.Monday, DayOfWeek.Monday]), check);
        ExpectDomainFailure(() => template.ReplaceSchedule([(DayOfWeek)7]), check);
        ExpectDomainFailure(() => template.ReplaceSchedule(null!), check);
        check(template.ScheduledDays.Count == 3, "invalid replacements preserve the previous schedule");

        template.ReplaceSchedule(Enum.GetValues<DayOfWeek>());
        check(template.ScheduledDays.Count == 7 && template.ScheduledDays.First() == DayOfWeek.Monday
              && template.ScheduledDays.Last() == DayOfWeek.Sunday, "all weekdays supported in display order");
        template.ReplaceSchedule([]);
        check(template.ScheduledDays.Count == 0, "empty schedule clears selected weekdays");

        var exercises = new FakeExerciseRepository();
        var request = new CreateWorkoutTemplateRequest("Workout", null, 2,
            [new WorkoutItem(exercises.Exercise.Id, 3, 10, 60, 0)]);
        var createValidator = new CreateWorkoutTemplateRequestValidator();
        check(createValidator.Validate(request).IsValid, "creation accepts an omitted schedule");
        check(createValidator.Validate(request with { ScheduledDays = [] }).IsValid,
            "creation accepts an empty schedule");
        check(!createValidator.Validate(request with { ScheduledDays = [(DayOfWeek)(-1)] }).IsValid,
            "creation rejects undefined weekdays");
        check(!createValidator.Validate(request with { ScheduledDays = [DayOfWeek.Friday, DayOfWeek.Friday] }).IsValid,
            "creation rejects duplicate weekdays");

        var repository = new FakeTemplateRepository();
        var unitOfWork = new FakeUnitOfWork();
        var currentUser = new FakeCurrentUser(userId);
        var create = new CreateWorkoutTemplateHandler(repository, exercises, currentUser, unitOfWork,
            createValidator, NullLogger<CreateWorkoutTemplateHandler>.Instance);
        var created = await create.ExecuteAsync(request with { ScheduledDays = [DayOfWeek.Friday, DayOfWeek.Monday] });
        check(created.TemplateId == repository.Template!.Id && unitOfWork.SaveCount == 1,
            "creation saves the scheduled template");
        check(repository.Template.ScheduledDays.SequenceEqual([DayOfWeek.Monday, DayOfWeek.Friday]),
            "creation maps selected weekdays into the domain");

        // Use a template without exercises to test detail mapping independently of exercise navigation loading.
        repository.Template = template;
        var update = new UpdateWorkoutTemplateScheduleHandler(repository, currentUser, unitOfWork,
            new UpdateWorkoutTemplateScheduleRequestValidator(),
            NullLogger<UpdateWorkoutTemplateScheduleHandler>.Instance);
        var response = await update.ExecuteAsync(template.Id,
            new UpdateWorkoutTemplateScheduleRequest([DayOfWeek.Friday, DayOfWeek.Tuesday, DayOfWeek.Thursday]));
        check(response.TemplateId == template.Id && response.ScheduledDays.SequenceEqual(
            [DayOfWeek.Tuesday, DayOfWeek.Thursday, DayOfWeek.Friday]), "schedule update returns replacement days");
        check(unitOfWork.SaveCount == 2, "schedule update saves exactly once");

        var details = await new GetWorkoutTemplateByIdHandler(repository,
            NullLogger<GetWorkoutTemplateByIdHandler>.Instance, currentUser).HandleAsync(template.Id);
        check(details.ScheduledDays.SequenceEqual(response.ScheduledDays), "template details include the schedule");

        await ExpectFailure<ValidationException>(() => update.ExecuteAsync(template.Id,
            new UpdateWorkoutTemplateScheduleRequest(null!)), check);
        await ExpectFailure<ValidationException>(() => update.ExecuteAsync(template.Id,
            new UpdateWorkoutTemplateScheduleRequest([DayOfWeek.Monday, DayOfWeek.Monday])), check);
        await ExpectFailure<ValidationException>(() => update.ExecuteAsync(template.Id,
            new UpdateWorkoutTemplateScheduleRequest([(DayOfWeek)100])), check);

        repository.Template = WorkoutTemplate.Create(Guid.NewGuid(), "Other user's workout", 1);
        await ExpectFailure<ForbiddenException>(() => update.ExecuteAsync(repository.Template.Id,
            new UpdateWorkoutTemplateScheduleRequest([DayOfWeek.Monday])), check);
        check(repository.Template.ScheduledDays.Count == 0, "forbidden update leaves the schedule unchanged");
        repository.Template = null;
        await ExpectFailure<NotFoundException>(() => update.ExecuteAsync(Guid.NewGuid(),
            new UpdateWorkoutTemplateScheduleRequest([])), check);
        check(unitOfWork.SaveCount == 2, "invalid, forbidden and missing-template requests do not save");

        repository.Template = template;
        var cleared = await update.ExecuteAsync(template.Id, new UpdateWorkoutTemplateScheduleRequest([]));
        check(cleared.ScheduledDays.Count == 0 && unitOfWork.SaveCount == 3, "clearing a schedule is persisted");

        var list = new GetWorkoutTemplatesHandler(repository, currentUser, new GetWorkoutTemplatesRequestValidator());
        await list.ExecuteAsync(new GetWorkoutTemplatesRequest(DayOfWeek.Sunday));
        check(repository.LastListUserId == userId && repository.LastListDay == DayOfWeek.Sunday,
            "list passes authenticated owner and weekday to repository");
        var all = await list.ExecuteAsync(new GetWorkoutTemplatesRequest());
        check(repository.LastListDay is null && all.Single().ScheduledDays.Count == 0,
            "unfiltered list includes unscheduled templates");
        await ExpectFailure<ValidationException>(() => list.ExecuteAsync(
            new GetWorkoutTemplatesRequest((DayOfWeek)7)), check);
    }

    private static void ExpectDomainFailure(Action action, Action<bool, string> check)
    {
        try
        {
            action();
            throw new Exception("Expected invalid schedule to fail.");
        }
        catch (DomainException)
        {
            check(true, "invalid schedule rejected by domain");
        }
    }

    private static async Task ExpectFailure<T>(Func<Task> action, Action<bool, string> check) where T : Exception
    {
        try
        {
            await action();
            throw new Exception($"Expected {typeof(T).Name}.");
        }
        catch (T)
        {
            check(true, $"request rejected with {typeof(T).Name}");
        }
    }

    private sealed class FakeCurrentUser(Guid userId) : ICurrentUser
    {
        public Guid UserId => userId;
    }

    private sealed class FakeUnitOfWork : IUnitOfWork
    {
        public int SaveCount { get; private set; }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SaveCount++;
            return Task.FromResult(1);
        }
    }

    private sealed class FakeTemplateRepository : IWorkoutTemplateRepository
    {
        public WorkoutTemplate? Template { get; set; }
        public Guid LastListUserId { get; private set; }
        public DayOfWeek? LastListDay { get; private set; }

        public void Add(WorkoutTemplate template) => Template = template;
        public Task<WorkoutTemplate?> GetByNameAsync(string name) => Task.FromResult(Template);
        public Task<bool> CheckUserOwnedByNameAsync(Guid userId, string name) => Task.FromResult(false);
        public Task<WorkoutTemplate?> GetByIdAsync(Guid templateId) => Task.FromResult(Template);
        public Task<WorkoutTemplate?> GetWithExercisesById(Guid templateId) => Task.FromResult(Template);

        public Task<IReadOnlyCollection<WorkoutTemplate>> GetAllByUserIdAsync(Guid userId, DayOfWeek? day = null)
        {
            LastListUserId = userId;
            LastListDay = day;
            return Task.FromResult<IReadOnlyCollection<WorkoutTemplate>>(Template is null ? [] : [Template]);
        }
    }

    private sealed class FakeExerciseRepository : IExerciseRepository
    {
        public Exercise Exercise { get; } = Exercise.Create("Push-up", ExerciseType.Bodyweight);

        public void Add(Exercise exercise) => throw new NotSupportedException();
        public Task<Exercise?> GetByNameAsync(string name) => Task.FromResult<Exercise?>(Exercise);
        public Task<Exercise?> GetByIdAsync(Guid id) => Task.FromResult<Exercise?>(Exercise);
        public Task<IReadOnlyCollection<Exercise>> GetAvailableByIdsAsync(Guid userId, ICollection<Guid> ids)
            => Task.FromResult<IReadOnlyCollection<Exercise>>([Exercise]);
    }
}
