using FluentValidation;

namespace Application.Features.Workout.Template;

public sealed class ScheduledDaysValidator : AbstractValidator<IReadOnlyCollection<DayOfWeek>>
{
    public ScheduledDaysValidator()
    {
        RuleFor(days => days)
            .Must(days => days.Distinct().Count() == days.Count)
            .WithMessage("Each weekday can only be scheduled once.");

        RuleForEach(days => days)
            .IsInEnum();
    }
}
