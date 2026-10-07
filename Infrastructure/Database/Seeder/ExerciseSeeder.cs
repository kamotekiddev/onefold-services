using Domain.Entities.Workout;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Seeder;

public static class ExerciseSeeder
{
    private static readonly (string Name, string Description)[] Exercises =
    [
        (
            "Barbell Bench Press",
            "Compound chest exercise primarily targeting the chest, shoulders, and triceps."
        ),
        (
            "Incline Dumbbell Press",
            "Compound pressing exercise emphasizing the upper chest, shoulders, and triceps."
        ),
        (
            "Seated Dumbbell Shoulder Press",
            "Compound shoulder pressing exercise targeting the deltoids and triceps."
        ),
        (
            "Dumbbell Lateral Raise",
            "Isolation exercise targeting the lateral deltoids."
        ),
        (
            "Cable Tricep Pushdown",
            "Isolation exercise targeting the triceps."
        )
    ];

    public static async Task SeedAsync(AppDbContext dbContext)
    {
        foreach (var (name, description) in Exercises)
        {
            var exists = await dbContext.Exercises
                .AnyAsync(x => x.Name == name && x.UserId == null);

            if (exists)
                continue;

            var exercise = Exercise.Create(name, description);
            exercise.Activate();

            dbContext.Exercises.Add(exercise);
        }

        await dbContext.SaveChangesAsync();
    }
}