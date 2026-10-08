using Domain.Entities.Workout.Session;
using Domain.Entities.Workout.Template;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Seeder;

public static class WorkoutTemplateSeeder
{
    private const string UserEmail = "dev@onefold.app";
    private const string TemplateName = "Push Day";

    public static async Task SeedAsync(AppDbContext dbContext)
    {
        var user = await dbContext.Users
            .FirstOrDefaultAsync(x => x.Email == UserEmail);

        if (user is null)
            throw new InvalidOperationException(
                $"Seed user '{UserEmail}' was not found.");

        var existingTemplate = await dbContext.WorkoutTemplates
            .FirstOrDefaultAsync(x =>
                x.UserId == user.Id &&
                x.Name == TemplateName);

        if (existingTemplate is not null)
            return;

        var exercises = await dbContext.Exercises
            .Where(x =>
                x.UserId == null &&
                (
                    x.Name == "Barbell Bench Press" ||
                    x.Name == "Incline Dumbbell Press" ||
                    x.Name == "Seated Dumbbell Shoulder Press" ||
                    x.Name == "Dumbbell Lateral Raise" ||
                    x.Name == "Cable Tricep Pushdown"
                ))
            .ToDictionaryAsync(x => x.Name);

        var template = WorkoutTemplate.Create(
            user.Id,
            TemplateName,
            restInMinutes: 2,
            description: "Chest, shoulders, and triceps workout.");

        template.AddExercise(
            WorkoutTemplateExercise.Create(
                template.Id,
                exercises["Barbell Bench Press"].Id,
                setCount: 3,
                targetReps: 8,
                restPerSetInSeconds: 120,
                sortIndex: 0,
                WeightUnit.Pounds));

        template.AddExercise(
            WorkoutTemplateExercise.Create(
                template.Id,
                exercises["Incline Dumbbell Press"].Id,
                setCount: 3,
                targetReps: 10,
                restPerSetInSeconds: 90,
                sortIndex: 1,
                WeightUnit.Pounds));

        template.AddExercise(
            WorkoutTemplateExercise.Create(
                template.Id,
                exercises["Seated Dumbbell Shoulder Press"].Id,
                setCount: 3,
                targetReps: 10,
                restPerSetInSeconds: 90,
                sortIndex: 2,
                WeightUnit.Pounds));

        template.AddExercise(
            WorkoutTemplateExercise.Create(
                template.Id,
                exercises["Dumbbell Lateral Raise"].Id,
                setCount: 3,
                targetReps: 12,
                restPerSetInSeconds: 60,
                sortIndex: 3,
                WeightUnit.Pounds));

        template.AddExercise(
            WorkoutTemplateExercise.Create(
                template.Id,
                exercises["Cable Tricep Pushdown"].Id,
                setCount: 3,
                targetReps: 12,
                restPerSetInSeconds: 60,
                sortIndex: 4,
                WeightUnit.Pounds));

        dbContext.WorkoutTemplates.Add(template);

        await dbContext.SaveChangesAsync();
    }
}