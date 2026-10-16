using FluentValidation;

namespace Application.Features.Workout.Template.GetWorkoutTemplates;

public sealed class GetWorkoutTemplatesRequestValidator : AbstractValidator<GetWorkoutTemplatesRequest>
{
    public GetWorkoutTemplatesRequestValidator()
    {
        RuleFor(x => x.Day).IsInEnum();
    }
}
