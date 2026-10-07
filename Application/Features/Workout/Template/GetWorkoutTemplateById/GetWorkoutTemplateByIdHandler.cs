using Application.Abstractions.Authentication;
using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using Application.Dtos;
using Microsoft.Extensions.Logging;

namespace Application.Features.Workout.Template.GetWorkoutTemplateById;

public class GetWorkoutTemplateByIdHandler(
    IWorkoutTemplateRepository workoutTemplateRepository,
    ILogger<GetWorkoutTemplateByIdHandler> logger,
    ICurrentUser currentUser)
{
    public async Task<WorkoutTemplateDto> HandleAsync(Guid templateId)
    {
        var userId = currentUser.UserId;

        var workoutTemplate = await workoutTemplateRepository.GetWithExercisesById(templateId);

        if (workoutTemplate is null)
        {
            logger.LogWarning("Invalid TemplateId:{templateId}", templateId);
            throw new NotFoundException("Invalid templateId.");
        }

        if (workoutTemplate.UserId != userId)
        {
            logger.LogWarning(
                "User:{userId} attempted to access WorkoutTemplateId:{templateId} that belong to User:{ownerId}",
                userId, templateId,
                workoutTemplate.UserId);
            throw new ForbiddenException("Invalid templateId.");
        }

        var workoutExercises = workoutTemplate.WorkoutExercises
            .Select(x =>
                new WorkoutExerciseDto(x.Id,
                    x.ExerciseId,
                    x.Exercise.Name,
                    x.TargetSet,
                    x.TargetReps,
                    x.SortIndex))
            .OrderBy(x => x.SortIndex)
            .ToList();

        return new WorkoutTemplateDto(
            workoutTemplate.Id,
            userId, workoutTemplate.Name,
            workoutTemplate.Description,
            workoutTemplate.RestInMinutes,
            workoutExercises
        );
    }
}