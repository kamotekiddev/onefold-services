using Application.Abstractions.Authentication;
using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using Domain.Entities.Workout.Session;

namespace Application.Features.Workout.Session.SaveSession;

public class SaveSessionHandler(ICurrentUser currentUser, IWorkoutTemplateRepository workoutTemplateRepository)
{
    public async Task<SaveSessionResponse> ExecuteAsync(SaveSessionRequest request)
    {
        var userId = currentUser.UserId;

        var template = await workoutTemplateRepository.GetByIdAsync(request.WorkoutTemplateId);
        if (template is null)
            throw new NotFoundException("Invalid TemplateId.");

        if (template.UserId != userId)
            throw new ForbiddenException("Template does not belong to user.");


        var session = WorkoutSession.Create(template.Id, userId, request.StartedAt, request.CompletedAt);
        var templateExercises = template.WorkoutExercises.ToDictionary(x => x.ExerciseId);

        foreach (var exerciseRequest in request.Exercises)
        {
            var templateExercise = templateExercises[exerciseRequest.ExerciseId];
            var sessionExercise = WorkoutSessionExercise.Create(
                session.Id,
                exerciseRequest.ExerciseId,
                templateExercise.SetCount,
                templateExercise.TargetRepsPerSet,
                templateExercise.RestPerSetInSeconds,
                templateExercise.OrderIndex);

            foreach (var set in exerciseRequest.Sets)
            {
                sessionExercise.AddSet(set.SetNumber, set.Reps, set.Weight);
            }

            session.AddExercise(sessionExercise);
        }

        throw new NotImplementedException();
    }
}