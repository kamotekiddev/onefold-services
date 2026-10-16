using Application.Abstractions.Authentication;
using Application.Abstractions.Persistence;
using Application.Dtos;
using FluentValidation;

namespace Application.Features.Workout.Template.GetWorkoutTemplates;

public sealed class GetWorkoutTemplatesHandler(
    IWorkoutTemplateRepository workoutTemplateRepository,
    ICurrentUser currentUser,
    IValidator<GetWorkoutTemplatesRequest> validator)
{
    public async Task<IReadOnlyCollection<WorkoutTemplateListItemDto>> ExecuteAsync(GetWorkoutTemplatesRequest request)
    {
        await validator.ValidateAndThrowAsync(request);

        var templates = await workoutTemplateRepository.GetAllByUserIdAsync(currentUser.UserId, request.Day);

        return templates.Select(template => new WorkoutTemplateListItemDto(
            template.Id,
            template.Name,
            template.Description,
            template.RestInMinutes,
            template.ScheduledDays)).ToList();
    }
}
