using Application.Abstractions.Authentication;
using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Application.Features.Workout.Template.UpdateWorkoutTemplateSchedule;

public sealed class UpdateWorkoutTemplateScheduleHandler(
    IWorkoutTemplateRepository workoutTemplateRepository,
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork,
    IValidator<UpdateWorkoutTemplateScheduleRequest> validator,
    ILogger<UpdateWorkoutTemplateScheduleHandler> logger)
{
    public async Task<UpdateWorkoutTemplateScheduleResponse> ExecuteAsync(
        Guid templateId, UpdateWorkoutTemplateScheduleRequest request)
    {
        await validator.ValidateAndThrowAsync(request);

        var template = await workoutTemplateRepository.GetByIdAsync(templateId);
        if (template is null)
            throw new NotFoundException("Invalid template id.");

        if (template.UserId != currentUser.UserId)
            throw new ForbiddenException("Invalid template id.");

        template.ReplaceSchedule(request.ScheduledDays);
        await unitOfWork.SaveChangesAsync();

        logger.LogInformation("Updated schedule for WorkoutTemplate: {WorkoutTemplateId}.", template.Id);

        return new UpdateWorkoutTemplateScheduleResponse(template.Id, template.ScheduledDays);
    }
}
