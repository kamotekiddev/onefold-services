using FluentValidation;

namespace Application.Features.Workout.Exercise.CreateExercise;

public sealed class CreateExerciseRequestValidator
    : AbstractValidator<CreateExerciseRequest>
{
    public CreateExerciseRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Type)
            .IsInEnum();

        RuleFor(x => x.Description)
            .MaximumLength(500);
    }
}