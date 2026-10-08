using FluentValidation;

namespace Application.Features.Workout.Session.SaveSession;

public sealed class SaveSessionRequestValidator
    : AbstractValidator<SaveSessionRequest>
{
    public SaveSessionRequestValidator()
    {
        RuleFor(x => x.WorkoutTemplateId)
            .NotEmpty();

        RuleFor(x => x.CompletedAt)
            .GreaterThan(x => x.StartedAt)
            .WithMessage("CompletedAt must be later than StartedAt.");

        RuleFor(x => x.Exercises)
            .NotEmpty()
            .Must(HaveUniqueExercises)
            .WithMessage("An exercise can only appear once in a workout session.");

        RuleForEach(x => x.Exercises)
            .SetValidator(new SaveSessionExerciseRequestValidator());
    }

    private static bool HaveUniqueExercises(
        IReadOnlyCollection<SaveSessionExerciseRequest> exercises)
    {
        return exercises
            .Select(x => x.ExerciseId)
            .Distinct()
            .Count() == exercises.Count;
    }
}

public sealed class SaveSessionExerciseRequestValidator
    : AbstractValidator<SaveSessionExerciseRequest>
{
    public SaveSessionExerciseRequestValidator()
    {
        RuleFor(x => x.ExerciseId)
            .NotEmpty();

        RuleFor(x => x.SortIndex)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.Sets)
            .NotEmpty()
            .Must(HaveUniqueSetNumbers)
            .WithMessage("Set numbers must be unique.");

        RuleForEach(x => x.Sets)
            .SetValidator(new SaveSessionWorkoutSetRequestValidator());
    }

    private static bool HaveUniqueSetNumbers(
        IReadOnlyCollection<SaveSessionWorkoutSetRequest> sets)
    {
        return sets
            .Select(x => x.SetNumber)
            .Distinct()
            .Count() == sets.Count;
    }
}

public sealed class SaveSessionWorkoutSetRequestValidator
    : AbstractValidator<SaveSessionWorkoutSetRequest>
{
    public SaveSessionWorkoutSetRequestValidator()
    {
        RuleFor(x => x.SetNumber)
            .GreaterThan(0);

        RuleFor(x => x.Reps)
            .GreaterThan(0);

        RuleFor(x => x.Weight)
            .GreaterThan(0)
            .When(x => x.Weight.HasValue);

        RuleFor(x => x.Unit)
            .NotNull()
            .When(x => x.Weight.HasValue)
            .WithMessage("Unit is required when weight is provided.");

        RuleFor(x => x.Weight)
            .NotNull()
            .When(x => x.Unit.HasValue)
            .WithMessage("Weight is required when unit is provided.");
    }
}