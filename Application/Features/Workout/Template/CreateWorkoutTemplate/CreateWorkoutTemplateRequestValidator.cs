using FluentValidation;

namespace Application.Features.Workout.Template.CreateWorkoutTemplate;

public sealed class CreateWorkoutTemplateRequestValidator
    : AbstractValidator<CreateWorkoutTemplateRequest>
{
    public CreateWorkoutTemplateRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Description)
            .MaximumLength(500);

        RuleFor(x => x.RestInMinutes)
            .GreaterThan(0);

        RuleFor(x => x.Exercises)
            .NotEmpty()
            .Must(HaveUniqueExercises)
            .WithMessage("An exercise can only be added once to a workout template.");

        RuleForEach(x => x.Exercises)
            .SetValidator(new WorkoutItemValidator());
    }

    private static bool HaveUniqueExercises(
        IReadOnlyCollection<WorkoutItem> exercises)
    {
        return exercises
            .Select(x => x.ExerciseId)
            .Distinct()
            .Count() == exercises.Count;
    }
}

public sealed class WorkoutItemValidator
    : AbstractValidator<WorkoutItem>
{
    public WorkoutItemValidator()
    {
        RuleFor(x => x.ExerciseId)
            .NotEmpty();

        RuleFor(x => x.TargetSet)
            .GreaterThan(0)
            .LessThanOrEqualTo(20);

        RuleFor(x => x.TargetReps)
            .GreaterThan(0)
            .LessThanOrEqualTo(100);

        RuleFor(x => x.RestInSeconds)
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(3600);

        RuleFor(x => x.SortIndex)
            .GreaterThanOrEqualTo(0);

        When(x => x.WeightIncrement.HasValue, () =>
        {
            RuleFor(x => x.WeightIncrement)
                .GreaterThan(0);
        });

        When(x => x.WeightUnit.HasValue, () =>
        {
            RuleFor(x => x.WeightIncrement)
                .NotNull()
                .WithMessage("Weight increment is required when weight unit is specified.");
        });

        When(x => x.WeightIncrement.HasValue, () =>
        {
            RuleFor(x => x.WeightUnit)
                .NotNull()
                .WithMessage("Weight unit is required when weight increment is specified.");
        });
    }
}