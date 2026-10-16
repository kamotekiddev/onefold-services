using FluentValidation;

namespace Application.Features.Workout.Template.UpdateWorkoutTemplateSchedule;

public sealed class UpdateWorkoutTemplateScheduleRequestValidator
    : AbstractValidator<UpdateWorkoutTemplateScheduleRequest>
{
    public UpdateWorkoutTemplateScheduleRequestValidator()
    {
        RuleFor(x => x.ScheduledDays)
            .NotNull()
            .SetValidator(new ScheduledDaysValidator());
    }
}
