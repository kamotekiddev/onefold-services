using FluentValidation;

namespace Application.Features.Workout.WorkoutTemplateModule.CreateWorkoutTemplate;

public sealed class CreateWorkoutTemplateValidator
    : AbstractValidator<CreateWorkoutTemplateRequest>
{
    public CreateWorkoutTemplateValidator()
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
            .Must(HaveUniqueExerciseIds)
            .WithMessage("Duplicate exercises are not allowed.");

        RuleForEach(x => x.Exercises)
            .SetValidator(new CreateWorkoutExerciseValidator());
    }

    private static bool HaveUniqueExerciseIds(
        IReadOnlyCollection<WorkoutItem> exercises)
    {
        var exerciseIds = exercises.Select(x => x.ExerciseId).ToList();
        return exerciseIds.Distinct().Count() == exerciseIds.Count;
    }
}

public sealed class CreateWorkoutExerciseValidator
    : AbstractValidator<WorkoutItem>
{
    public CreateWorkoutExerciseValidator()
    {
        RuleFor(x => x.ExerciseId)
            .NotEmpty();

        RuleFor(x => x.TargetReps)
            .GreaterThan(0);

        RuleFor(x => x.RestPerSetInSeconds)
            .GreaterThanOrEqualTo(0);
    }
}