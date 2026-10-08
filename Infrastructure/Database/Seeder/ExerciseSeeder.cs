using Domain.Entities.Workout;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Seeder;

public static class ExerciseSeeder
{
    private static readonly (string Name, string Description, ExerciseType Type)[] Exercises =
    [
        (
            "Barbell Bench Press",
            "Compound chest exercise primarily targeting the chest, shoulders, and triceps.",
            ExerciseType.Weighted
        ),
        (
            "Incline Dumbbell Press",
            "Compound pressing exercise emphasizing the upper chest, shoulders, and triceps.",
            ExerciseType.Weighted
        ),
        (
            "Seated Dumbbell Shoulder Press",
            "Compound shoulder pressing exercise targeting the deltoids and triceps.",
            ExerciseType.Weighted
        ),
        (
            "Dumbbell Lateral Raise",
            "Isolation exercise targeting the lateral deltoids.",
            ExerciseType.Weighted
        ),
        (
            "Cable Tricep Pushdown",
            "Isolation exercise targeting the triceps.",
            ExerciseType.Weighted
        )
    ];

    public static async Task SeedAsync(AppDbContext dbContext)
    {
        foreach (var (name, description, type) in Exercises)
        {
            var exists = await dbContext.Exercises
                .AnyAsync(x => x.Name == name && x.UserId == null);

            if (exists)
                continue;

            var exercise = Exercise.Create(name, type, description);
            exercise.Activate();

            dbContext.Exercises.Add(exercise);
        }

        await dbContext.SaveChangesAsync();
    }
}